using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Trip
{
    // DTO for creating a new Trip
    public class CreateTripDto
    {
        [Required(ErrorMessage = "Route ID is required")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Company ID is required")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Driver ID is required")]
        public string DriverId { get; set; } = string.Empty;

        public int? BusId { get; set; }

        [Required(ErrorMessage = "Main departure time is required")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Available seats is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Available seats must be greater than 0")]
        public int AvailableSeats { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        // List of stations with their scheduled times
        public List<TripStationDto> TripStations { get; set; } = new List<TripStationDto>();
    }

    // DTO for Trip details in responses
    public class TripDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string DriverId { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string DriverPhoneNumber { get; set; } = string.Empty; // Added driver phone number
        public int? BusId { get; set; }
        public string BusRegistrationNumber { get; set; } = string.Empty;
        public string AmenityDescription { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int AvailableSeats { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<TripStationResponseDto> TripStations { get; set; } = new List<TripStationResponseDto>();
    }

    // DTO for TripStation input (used in Create)
    public class TripStationDto
    {
        [Required(ErrorMessage = "Station ID is required")]
        public int StationId { get; set; }

        [Required(ErrorMessage = "Sequence number is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Sequence number must be greater than 0")]
        public int SequenceNumber { get; set; }

        // For the first station (start station), ArrivalTime can be null
        public DateTime? ArrivalTime { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }
    }

    // DTO for TripStation in responses
    public class TripStationResponseDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int SequenceNumber { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
    }

    // DTO for Paginated Trips response
    public class PaginatedTripDto
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<TripDto> Trips { get; set; } = new List<TripDto>();
    }

    // DTO for updating Trip status
    public class UpdateTripStatusDto
    {
        public int TripId { get; set; }
        public bool IsCompleted { get; set; }
    }

    // DTO for Trip Search
    public class SearchTripDto
    {
        [Required(ErrorMessage = "Start city ID is required")]
        public int StartCityId { get; set; }
        
        [Required(ErrorMessage = "End city ID is required")]
        public int EndCityId { get; set; }
        
        [Required(ErrorMessage = "Departure date is required")]
        public DateTime DepartureDate { get; set; }
        
        [Required(ErrorMessage = "Number of required seats is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Required seats must be greater than 0")]
        public int RequiredSeats { get; set; }
    }
}