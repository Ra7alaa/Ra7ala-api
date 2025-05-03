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
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.ChatBotService
{
    public class ChatBotService : IChatBotService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAISettings _openAISettings;
        private readonly ITripService _tripService;
        private readonly ILogger<ChatBotService> _logger;
        
        public ChatBotService(
            HttpClient httpClient,
            IOptions<OpenAISettings> openAISettings,
            ITripService tripService,
            ILogger<ChatBotService> logger)
        {
            _httpClient = httpClient;
            _openAISettings = openAISettings.Value;
            _tripService = tripService;
            _logger = logger;
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAISettings.ApiKey);
        }

        public async Task<ServiceResult<ChatBotResponseDto>> ProcessChatMessageAsync(ChatBotRequestDto request)
        {
            try
            {
                // Get all future trips to have context for the chatbot
                var tripsResult = await _tripService.GetAllFutureTripsAsync();
                if (!tripsResult.IsSuccess)
                {
                    return ServiceResult<ChatBotResponseDto>.Failure("Failed to retrieve trip information: " + string.Join(", ", tripsResult.Errors));
                }

                string tripContext = BuildTripContext(tripsResult.Data);

                // Prepare the messages for the chatbot
                var messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "system", Content = $"You are a helpful customer service assistant for Ra7ala, a transportation service. Your role is to help customers with their inquiries about trips. Here's the current information about our trips: {tripContext}" },
                    new ChatMessage { Role = "user", Content = request.UserMessage }
                };

                var requestBody = new ChatBotOpenAIRequestDto
                {
                    Model = _openAISettings.Model,
                    Messages = messages
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody, jsonOptions),
                    Encoding.UTF8,
                    "application/json");

                // Send request to OpenAI API with retries
                HttpResponseMessage response = null;
                int retries = 0;
                while (retries <= _openAISettings.MaxRetries)
                {
                    try
                    {
                        response = await _httpClient.PostAsync(_openAISettings.Endpoint, content);
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
                        _logger.LogError(ex, "Error occurred while sending request to OpenAI API. Attempt {Attempt}", retries + 1);
                        retries++;
                        if (retries > _openAISettings.MaxRetries)
                            throw;
                        await Task.Delay(1000 * retries);
                    }
                }

                // Process the response
                if (response == null || !response.IsSuccessStatusCode)
                {
                    string errorMessage = response != null ? 
                        $"OpenAI API returned status code {response.StatusCode}: {await response.Content.ReadAsStringAsync()}" : 
                        "Failed to get a response from OpenAI API after retries";
                    
                    _logger.LogError(errorMessage);
                    return ServiceResult<ChatBotResponseDto>.Failure(errorMessage);
                }

                var apiResponse = await JsonSerializer.DeserializeAsync<ChatBotOpenAIResponseDto>(
                    await response.Content.ReadAsStreamAsync(), jsonOptions);

                if (apiResponse == null || apiResponse.Choices == null || apiResponse.Choices.Count == 0)
                {
                    return ServiceResult<ChatBotResponseDto>.Failure("Invalid or empty response from OpenAI API");
                }

                var chatbotResponse = new ChatBotResponseDto
                {
                    Response = apiResponse.Choices[0].Message.Content,
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
            sb.AppendLine("Available trips information:");
            
            foreach (var trip in trips)
            {
                sb.AppendLine($"Trip ID: {trip.Id}");
                sb.AppendLine($"From: {(trip.TripStations.Count > 0 ? trip.TripStations[0].CityName : "Unknown")} " +
                              $"To: {(trip.TripStations.Count > 0 ? trip.TripStations[trip.TripStations.Count - 1].CityName : "Unknown")}");
                sb.AppendLine($"Company: {trip.CompanyName}");
                sb.AppendLine($"Departure: {trip.DepartureTime:yyyy-MM-dd HH:mm}");
                sb.AppendLine($"Arrival: {trip.ArrivalTime?.ToString("yyyy-MM-dd HH:mm") ?? "Not specified"}");
                sb.AppendLine($"Price: {trip.Price:C}");
                sb.AppendLine($"Available Seats: {trip.AvailableSeats}");
                sb.AppendLine($"Bus Info: {trip.BusRegistrationNumber}, {trip.AmenityDescription}");
                sb.AppendLine($"Route: {trip.RouteName}");
                sb.AppendLine("Stops:");
                
                foreach (var station in trip.TripStations)
                {
                    sb.AppendLine($"  - {station.StationName}, {station.CityName}, " +
                                 $"Arrival: {station.ArrivalTime?.ToString("HH:mm") ?? "N/A"}, " +
                                 $"Departure: {station.DepartureTime:HH:mm}");
                }
                
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}