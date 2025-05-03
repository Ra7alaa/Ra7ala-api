using Application.DTOs.ChatBot;
using Application.DTOs.Trip;
using Application.Models;
using Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.ChatBotService
{
    public class ChatBotService : IChatBotService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _geminiSettings;
        private readonly ITripService _tripService;
        private readonly ILogger<ChatBotService> _logger;

        public ChatBotService(
            HttpClient httpClient,
            IOptions<GeminiSettings> geminiSettings,
            ITripService tripService,
            ILogger<ChatBotService> logger)
        {
            _httpClient = httpClient;
            _geminiSettings = geminiSettings.Value;
            _tripService = tripService;
            _logger = logger;
        }

        public async Task<ServiceResult<ChatBotResponseDto>> ProcessMessageAsync(ChatBotRequestDto request)
        {
            try
            {
                // Get all future trips to provide context for the chatbot
                var tripsResult = await _tripService.GetAllFutureTripsAsync();
                if (!tripsResult.IsSuccess)
                {
                    return ServiceResult<ChatBotResponseDto>.Failure("Failed to retrieve trip information: " + string.Join(", ", tripsResult.Errors));
                }

                // Build trip context for the AI
                string tripContext = BuildTripContext(tripsResult.Data);

                // Create Gemini API request
                var systemPrompt = $@"You are a helpful customer service assistant for Ra7ala, a transportation service. Your role is to help customers with their inquiries about trips.

                                            Instructions:
                                            1. Detect the user's language and dialect from their message and respond in the same language and dialect (e.g., if they use Egyptian Arabic, respond in Egyptian Arabic; if they use English, respond in English).
                                            2. Be concise and direct - keep your responses under 3 sentences whenever possible.
                                            3. Always include specific details like trip IDs, times, prices, and available seats.
                                            4. If multiple trips match a query, mention only the most relevant 2-3 options.
                                            5. When mentioning times, format them clearly for easy reading.
                                            6. Only answer queries related to trip schedules, routes, stations, prices, amenities, and bus details.
                                            7. When responding in Arabic, format numbers and currency according to Arabic conventions.
                                            8. Currency should be in EGP (Egyptian Pounds).
                                            
                                            Here's the full context about all available trips:

                                            {tripContext}";
                
                var userQuestion = request.Message;

                var geminiRequest = new GeminiRequestDto
                {
                    Contents = new List<GeminiContentDto>
                    {
                        new GeminiContentDto
                        {
                            Parts = new List<GeminiPartDto>
                            {
                                new GeminiPartDto { Text = systemPrompt },
                                new GeminiPartDto { Text = userQuestion }
                            }
                        }
                    }
                };

                // Prepare for API call
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(geminiRequest, jsonOptions),
                    Encoding.UTF8,
                    "application/json");

                // Build the API URL with API key
                var apiUrl = $"{_geminiSettings.Endpoint}?key={_geminiSettings.ApiKey}";

                // Send request to Gemini API with retries
                HttpResponseMessage response = null;
                int retries = 0;
                while (retries <= _geminiSettings.MaxRetries)
                {
                    try
                    {
                        response = await _httpClient.PostAsync(apiUrl, content);
                        if (response.IsSuccessStatusCode)
                            break;

                        // If we get rate limited, wait and retry
                        if ((int)response.StatusCode == 429)
                        {
                            retries++;
                            await Task.Delay(1000 * retries); // Exponential backoff
                            continue;
                        }

                        // For other errors, break and handle below
                        break;
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex, "Error occurred while sending request to Gemini API. Attempt {Attempt}", retries + 1);
                        retries++;
                        if (retries > _geminiSettings.MaxRetries)
                            throw;
                        await Task.Delay(1000 * retries);
                    }
                }

                // Process the response
                if (response == null || !response.IsSuccessStatusCode)
                {
                    string errorMessage = response != null ? 
                        $"Gemini API returned status code {response.StatusCode}: {await response.Content.ReadAsStringAsync()}" : 
                        "Failed to get a response from Gemini API after retries";
                    
                    _logger.LogError(errorMessage);
                    return ServiceResult<ChatBotResponseDto>.Failure(errorMessage);
                }

                // Deserialize response
                var responseJson = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<GeminiResponseDto>(responseJson, jsonOptions);

                if (apiResponse == null || apiResponse.Candidates == null || apiResponse.Candidates.Count == 0 ||
                    apiResponse.Candidates[0].Content == null || apiResponse.Candidates[0].Content.Parts == null || !apiResponse.Candidates[0].Content.Parts.Any())
                {
                    return ServiceResult<ChatBotResponseDto>.Failure("Invalid or empty response from Gemini API");
                }

                // Create response DTO
                var chatbotResponse = new ChatBotResponseDto
                {
                    Response = apiResponse.Candidates[0].Content.Parts.FirstOrDefault()?.Text ?? "No response content",
                    SessionId = request.SessionId,
                    Success = true
                };

                return ServiceResult<ChatBotResponseDto>.Success(chatbotResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return ServiceResult<ChatBotResponseDto>.Failure($"Error processing chat message: {ex.Message}");
            }
        }

        private string BuildTripContext(IEnumerable<TripDto> trips)
        {
            StringBuilder sb = new StringBuilder();
            
            // Group trips by route for better organization
            var tripGroups = trips.GroupBy(t => t.RouteId);
            
            foreach (var tripGroup in tripGroups)
            {
                var firstTrip = tripGroup.First();
                
                // Get route information from first trip
                string routeInfo = $"Route: {firstTrip.RouteName} (from {GetOriginCity(firstTrip)} to {GetDestinationCity(firstTrip)})";
                sb.AppendLine(routeInfo);
                
                // Add trips for this route
                foreach (var trip in tripGroup)
                {
                    // English version
                    sb.AppendLine($"Trip ID: {trip.Id}");
                    sb.AppendLine($"Route ID: {trip.RouteId}");
                    sb.AppendLine($"Route Name: {trip.RouteName}");
                    sb.AppendLine($"Departure Time: {trip.DepartureTime:yyyy-MM-dd HH:mm}");
                    sb.AppendLine($"Arrival Time: {(trip.ArrivalTime.HasValue ? trip.ArrivalTime.Value.ToString("yyyy-MM-dd HH:mm") : "Not specified")}");
                    sb.AppendLine($"Driver: {trip.DriverName} (ID: {trip.DriverId})");
                    sb.AppendLine($"Driver Phone: {trip.DriverPhoneNumber}");
                    sb.AppendLine($"Bus ID: {trip.BusId}");
                    sb.AppendLine($"Bus Registration: {trip.BusRegistrationNumber}");
                    sb.AppendLine($"Amenities: {trip.AmenityDescription}");
                    sb.AppendLine($"Trip Status: {(trip.IsCompleted ? "Completed" : "Scheduled")}");
                    sb.AppendLine($"Available Seats: {trip.AvailableSeats}");
                    sb.AppendLine($"Company: {trip.CompanyName} (ID: {trip.CompanyId})");
                    sb.AppendLine($"Price: {trip.Price} EGP");
                    
                    // Add stations information
                    sb.AppendLine("Stations:");
                    
                    // Add all stations in the trip
                    var orderedStations = trip.TripStations.OrderBy(s => s.SequenceNumber).ToList();
                    
                    for (int i = 0; i < orderedStations.Count; i++)
                    {
                        var station = orderedStations[i];
                        string stationType = i == 0 ? "Departure" : (i == orderedStations.Count - 1 ? "Arrival" : "Stop");
                        
                        sb.AppendLine($"  - {stationType}: {station.StationName}, {station.CityName}");
                        
                        if (i == 0) // First station
                        {
                            sb.AppendLine($"     Departs: {station.DepartureTime:HH:mm} on {station.DepartureTime:yyyy-MM-dd}");
                        }
                        else if (i == orderedStations.Count - 1) // Last station
                        {
                            sb.AppendLine($"     Arrives: {(station.ArrivalTime.HasValue ? station.ArrivalTime.Value.ToString("HH:mm") : "Not specified")} on {(station.ArrivalTime.HasValue ? station.ArrivalTime.Value.ToString("yyyy-MM-dd") : "Not specified")}");
                        }
                        else // Intermediate station
                        {
                            sb.AppendLine($"     Arrives: {(station.ArrivalTime.HasValue ? station.ArrivalTime.Value.ToString("HH:mm") : "Not specified")} on {(station.ArrivalTime.HasValue ? station.ArrivalTime.Value.ToString("yyyy-MM-dd") : "Not specified")}");
                            sb.AppendLine($"     Departs: {station.DepartureTime:HH:mm} on {station.DepartureTime:yyyy-MM-dd}");
                        }
                    }
                    
                    sb.AppendLine(new string('-', 40));
                }
                
                sb.AppendLine();
            }

            return sb.ToString();
        }
        
        private string GetOriginCity(TripDto trip)
        {
            return trip.TripStations
                .OrderBy(s => s.SequenceNumber)
                .FirstOrDefault()?.CityName ?? "Unknown";
        }
        
        private string GetDestinationCity(TripDto trip)
        {
            return trip.TripStations
                .OrderByDescending(s => s.SequenceNumber)
                .FirstOrDefault()?.CityName ?? "Unknown";
        }
    }
}