using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    // DTO for creating a new Route
    public class CreateRouteDto
    {
        [Required(ErrorMessage = "Company ID is required")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Route name is required")]
        [StringLength(100, ErrorMessage = "Route name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Start city ID is required")]
        public int StartCityId { get; set; }

        [Required(ErrorMessage = "End city ID is required")]
        public int EndCityId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Distance must be greater than 0")]
        public int Distance { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Estimated duration must be greater than 0")]
        public int EstimatedDuration { get; set; }

        public List<RouteStationDto> StartCityStationIds { get; set; } = new List<RouteStationDto>();
        public List<RouteStationDto> EndCityStationIds { get; set; } = new List<RouteStationDto>();
    }

    // DTO for updating an existing Route
    public class UpdateRouteDto
    {
        [Required(ErrorMessage = "Route ID is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Route name is required")]
        [StringLength(100, ErrorMessage = "Route name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Start city ID is required")]
        public int StartCityId { get; set; }

        [Required(ErrorMessage = "End city ID is required")]
        public int EndCityId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Distance must be greater than 0")]
        public int Distance { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Estimated duration must be greater than 0")]
        public int EstimatedDuration { get; set; }

        public List<RouteStationDto> StartCityStationIds { get; set; } = new List<RouteStationDto>();
        public List<RouteStationDto> EndCityStationIds { get; set; } = new List<RouteStationDto>();
    }

    // DTO for Route details in responses
    public class RouteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StartCityId { get; set; }
        public string StartCityName { get; set; }
        public int EndCityId { get; set; }
        public string EndCityName { get; set; }
        public int Distance { get; set; }
        public int EstimatedDuration { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RouteStationResponseDto> RouteStations { get; set; } = new List<RouteStationResponseDto>();
    }

    // DTO for RouteStation input (used in Create/Update)
    public class RouteStationDto
    {
        [Required(ErrorMessage = "Station ID is required")]
        public int StationId { get; set; }

        [Required(ErrorMessage = "Sequence number is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Sequence number must be greater than 0")]
        public int SequenceNumber { get; set; }
    }

    // DTO for RouteStation in responses
    public class RouteStationResponseDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int SequenceNumber { get; set; }
    }

    // DTO for Paginated Routes response
    public class PaginatedRouteDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<RouteDto> Routes { get; set; } = new List<RouteDto>();
    }
}