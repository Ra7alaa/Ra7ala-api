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
                string tripContext = BuildTripContext(tripsResult.Data ?? Enumerable.Empty<TripDto>());

                // Create Gemini API request
                var systemPrompt = $@"You are Ra7ala's customer service bot for a bus transportation service in Egypt.
Rules:
- Detect the user's language/dialect and respond in the exact same language and dialect.
- Give helpful and detailed answers (5-7 sentences), including trip ID, time, price, available seats, and amenities.
- If multiple trips match, list the top 2-3 options with their details.
- Only answer questions about trips, routes, prices, seats, and amenities. Currency is EGP.

Available trips:
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
                HttpResponseMessage? response = null;
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

                if (apiResponse == null || apiResponse.Candidates == null || apiResponse.Candidates.Count == 0)
                {
                    return ServiceResult<ChatBotResponseDto>.Failure("Invalid or empty response from Gemini API");
                }

                var firstCandidate = apiResponse.Candidates[0];
                if (firstCandidate.Content == null || firstCandidate.Content.Parts == null || !firstCandidate.Content.Parts.Any())
                {
                    return ServiceResult<ChatBotResponseDto>.Failure("Invalid or empty response from Gemini API");
                }

                // Create response DTO
                var chatbotResponse = new ChatBotResponseDto
                {
                    Response = firstCandidate.Content.Parts.FirstOrDefault()?.Text ?? "No response content",
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
            var tripList = trips.Take(30).ToList(); // Limit to 30 trips max
            if (!tripList.Any()) return "No upcoming trips available.";

            var sb = new StringBuilder();
            sb.AppendLine("Available upcoming trips (summary):");

            foreach (var trip in tripList)
            {
                var from = GetOriginCity(trip);
                var to   = GetDestinationCity(trip);
                sb.AppendLine(
                    $"[ID:{trip.Id}] {from}→{to} | " +
                    $"Dep:{trip.DepartureTime:MM-dd HH:mm} | " +
                    $"Price:{trip.Price}EGP | " +
                    $"Seats:{trip.AvailableSeats} | " +
                    $"Co:{trip.CompanyName} | " +
                    $"Bus:{trip.BusRegistrationNumber} | " +
                    $"Amenities:{trip.AmenityDescription}");
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