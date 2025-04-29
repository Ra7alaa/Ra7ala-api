using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Models;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Route
{
    public class RouteService : IRouteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RouteService> _logger;

        public RouteService(
            IUnitOfWork unitOfWork,
            ILogger<RouteService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult<RouteDto>> CreateRouteAsync(CreateRouteDto createDto)
        {
            try
            {
                // Basic validations
                if (createDto.StartCityId == createDto.EndCityId)
                    return ServiceResult<RouteDto>.Failure("Start and end cities cannot be the same");

                if (createDto.Distance <= 0)
                    return ServiceResult<RouteDto>.Failure("Distance must be greater than zero");

                if (createDto.EstimatedDuration <= 0)
                    return ServiceResult<RouteDto>.Failure("Estimated duration must be greater than zero");

                // Verify company exists
                var company = await _unitOfWork.CompanyRepository.GetCompanyByIdAsync(createDto.CompanyId);
                if (company == null)
                    return ServiceResult<RouteDto>.Failure($"Company with ID {createDto.CompanyId} not found");

                // Verify cities exist
                var startCity = await _unitOfWork.Cities.GetByIdAsync(createDto.StartCityId);
                if (startCity == null)
                    return ServiceResult<RouteDto>.Failure($"Start city with ID {createDto.StartCityId} not found");

                var endCity = await _unitOfWork.Cities.GetByIdAsync(createDto.EndCityId);
                if (endCity == null)
                    return ServiceResult<RouteDto>.Failure($"End city with ID {createDto.EndCityId} not found");

                // Verify stations and their cities
                foreach (var stationDto in createDto.StartCityStationIds)
                {
                    if (stationDto.SequenceNumber <= 0)
                        return ServiceResult<RouteDto>.Failure($"Sequence number for station {stationDto.StationId} must be greater than 0");

                    var isValid = await _unitOfWork.Routes.IsStationInCityAsync(stationDto.StationId, createDto.StartCityId);
                    if (!isValid)
                        return ServiceResult<RouteDto>.Failure($"Station with ID {stationDto.StationId} does not belong to city with ID {createDto.StartCityId}");
                }

                foreach (var stationDto in createDto.EndCityStationIds)
                {
                    if (stationDto.SequenceNumber <= 0)
                        return ServiceResult<RouteDto>.Failure($"Sequence number for station {stationDto.StationId} must be greater than 0");

                    var isValid = await _unitOfWork.Routes.IsStationInCityAsync(stationDto.StationId, createDto.EndCityId);
                    if (!isValid)
                        return ServiceResult<RouteDto>.Failure($"Station with ID {stationDto.StationId} does not belong to city with ID {createDto.EndCityId}");
                }

                // Verify unique sequence numbers
                var allSequenceNumbers = createDto.StartCityStationIds.Select(s => s.SequenceNumber)
                    .Concat(createDto.EndCityStationIds.Select(s => s.SequenceNumber + createDto.StartCityStationIds.Count))
                    .ToList();
                if (allSequenceNumbers.Distinct().Count() != allSequenceNumbers.Count)
                    return ServiceResult<RouteDto>.Failure("Sequence numbers must be unique across all stations");

                // Create new route
                var route = new Domain.Entities.Route
                {
                    Name = createDto.Name,
                    StartCityId = createDto.StartCityId,
                    EndCityId = createDto.EndCityId,
                    Distance = createDto.Distance,
                    EstimatedDuration = createDto.EstimatedDuration,
                    CompanyId = createDto.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                };

                await _unitOfWork.Routes.AddAsync(route);
                await _unitOfWork.SaveChangesAsync();

                // Add route stations
                var routeStations = new List<RouteStation>();

                foreach (var stationDto in createDto.StartCityStationIds)
                {
                    routeStations.Add(new RouteStation
                    {
                        RouteId = route.Id,
                        StationId = stationDto.StationId,
                        SequenceNumber = stationDto.SequenceNumber,
                        IsActive = true,
                        IsDeleted = false
                    });
                }

                foreach (var stationDto in createDto.EndCityStationIds)
                {
                    var sequenceOffset = createDto.StartCityStationIds.Count;
                    routeStations.Add(new RouteStation
                    {
                        RouteId = route.Id,
                        StationId = stationDto.StationId,
                        SequenceNumber = sequenceOffset + stationDto.SequenceNumber,
                        IsActive = true,
                        IsDeleted = false
                    });
                }

                await _unitOfWork.RouteStations.AddRangeAsync(routeStations);
                await _unitOfWork.SaveChangesAsync();

                // Get created route with details
                var createdRoute = await _unitOfWork.Routes.GetRouteByIdWithDetailsAsync(route.Id);
                if (createdRoute == null)
                    return ServiceResult<RouteDto>.Failure("Failed to retrieve created route");

                var routeDto = MapToRouteDto(createdRoute);
                return ServiceResult<RouteDto>.Success(routeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating route for company {CompanyId}. Details: {Message}, StackTrace: {StackTrace}",
                    createDto.CompanyId, ex.Message, ex.StackTrace);
                return ServiceResult<RouteDto>.Failure($"An unexpected error occurred while creating the route: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PaginatedRouteDto>> GetPaginatedRoutesAsync(int companyId, int pageNumber, int pageSize)
        {
            try
            {
                // Validate input
                if (companyId <= 0)
                    return ServiceResult<PaginatedRouteDto>.Failure("Company ID must be greater than 0");
                if (pageNumber < 1)
                    return ServiceResult<PaginatedRouteDto>.Failure("Page number must be greater than 0");
                if (pageSize < 1)
                    return ServiceResult<PaginatedRouteDto>.Failure("Page size must be greater than 0");

                // Verify company exists
                var company = await _unitOfWork.CompanyRepository.GetCompanyByIdAsync(companyId);
                if (company == null)
                    return ServiceResult<PaginatedRouteDto>.Failure($"Company with ID {companyId} not found");

                // Get paginated routes
                var paginatedResult = await _unitOfWork.Routes.GetPaginatedRoutesByCompanyIdAsync(companyId, pageNumber, pageSize);
                var routeDtos = paginatedResult.Routes.Select(MapToRouteDto).ToList();

                var paginatedDto = new PaginatedRouteDto
                {
                    TotalCount = paginatedResult.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Routes = routeDtos
                };

                return ServiceResult<PaginatedRouteDto>.Success(paginatedDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving paginated routes for company {CompanyId}. Details: {Message}, StackTrace: {StackTrace}",
                    companyId, ex.Message, ex.StackTrace);
                return ServiceResult<PaginatedRouteDto>.Failure($"An unexpected error occurred while retrieving routes: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<RouteDto>>> GetAllRoutesAsync(int companyId)
        {
            try
            {
                // Validate input
                if (companyId <= 0)
                    return ServiceResult<IEnumerable<RouteDto>>.Failure("Company ID must be greater than 0");

                // Verify company exists
                var company = await _unitOfWork.CompanyRepository.GetCompanyByIdAsync(companyId);
                if (company == null)
                    return ServiceResult<IEnumerable<RouteDto>>.Failure($"Company with ID {companyId} not found");

                // Get routes
                var routes = await _unitOfWork.Routes.GetRoutesByCompanyIdAsync(companyId);
                var routeDtos = routes.Select(MapToRouteDto).ToList();

                return ServiceResult<IEnumerable<RouteDto>>.Success(routeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving routes for company {CompanyId}. Details: {Message}, StackTrace: {StackTrace}",
                    companyId, ex.Message, ex.StackTrace);
                return ServiceResult<IEnumerable<RouteDto>>.Failure($"An unexpected error occurred while retrieving routes: {ex.Message}");
            }
        }

        public async Task<ServiceResult<RouteDto>> GetRouteByIdAsync(int routeId)
        {
            try
            {
                // Validate input
                if (routeId <= 0)
                    return ServiceResult<RouteDto>.Failure("Route ID must be greater than 0");

                // Get route
                var route = await _unitOfWork.Routes.GetRouteByIdWithDetailsAsync(routeId);
                if (route == null)
                    return ServiceResult<RouteDto>.Failure($"Route with ID {routeId} not found");

                var routeDto = MapToRouteDto(route);
                return ServiceResult<RouteDto>.Success(routeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving route {RouteId}. Details: {Message}, StackTrace: {StackTrace}",
                    routeId, ex.Message, ex.StackTrace);
                return ServiceResult<RouteDto>.Failure($"An unexpected error occurred while retrieving the route: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteRouteAsync(int routeId)
        {
            try
            {
                // Validate input
                if (routeId <= 0)
                    return ServiceResult.Failure("Route ID must be greater than 0");

                // Get route
                var route = await _unitOfWork.Routes.GetByIdAsync(routeId);
                if (route == null)
                    return ServiceResult.Failure($"Route with ID {routeId} not found");

                // Verify company exists
                var company = await _unitOfWork.CompanyRepository.GetCompanyByIdAsync(route.CompanyId);
                if (company == null)
                    return ServiceResult.Failure($"Company with ID {route.CompanyId} not found");

                // Soft delete route
                _unitOfWork.Routes.SoftDelete(route);

                // Soft delete associated route stations
                var routeStations = await _unitOfWork.RouteStations.GetAllAsync();
                var stationsToDelete = routeStations.Where(rs => rs.RouteId == routeId && !rs.IsDeleted).ToList();
                foreach (var station in stationsToDelete)
                {
                    _unitOfWork.RouteStations.SoftDelete(station);
                }

                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting route {RouteId}. Details: {Message}, StackTrace: {StackTrace}",
                    routeId, ex.Message, ex.StackTrace);
                return ServiceResult.Failure($"An unexpected error occurred while deleting the route: {ex.Message}");
            }
        }

        private RouteDto MapToRouteDto(Domain.Entities.Route route)
        {
            return new RouteDto
            {
                Id = route.Id,
                Name = route.Name,
                StartCityId = route.StartCityId,
                StartCityName = route.StartCity?.Name ?? "Unknown",
                EndCityId = route.EndCityId,
                EndCityName = route.EndCity?.Name ?? "Unknown",
                Distance = route.Distance,
                EstimatedDuration = route.EstimatedDuration,
                CompanyId = route.CompanyId,
                CompanyName = route.Company?.Name ?? "Unknown",
                IsActive = route.IsActive,
                CreatedAt = route.CreatedAt,
                RouteStations = route.RouteStations
                    .OrderBy(rs => rs.SequenceNumber)
                    .Select(rs => new RouteStationResponseDto
                    {
                        StationId = rs.StationId,
                        StationName = rs.Station?.Name ?? "Unknown",
                        CityId = rs.Station?.CityId ?? 0,
                        CityName = rs.Station?.City?.Name ?? "Unknown",
                        SequenceNumber = rs.SequenceNumber
                    }).ToList()
            };
        }

        public Task<ServiceResult<RouteDto>> UpdateRouteAsync(UpdateRouteDto updateDto)
        {
            throw new NotImplementedException();
        }
    }
}