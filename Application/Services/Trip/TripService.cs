using Application.DTOs.Trip;
using Application.Models;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Trip
{
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TripService> _logger;

        public TripService(IUnitOfWork unitOfWork, ILogger<TripService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult<TripDto>> CreateTripAsync(CreateTripDto createDto)
        {
            try
            {
                // Validate route exists
                var route = await _unitOfWork.Routes.GetRouteByIdWithDetailsAsync(createDto.RouteId);
                if (route == null)
                    return ServiceResult<TripDto>.Failure($"Route with ID {createDto.RouteId} not found.");

                // Validate company exists
                var company = await _unitOfWork.CompanyRepository.GetByIdAsync(createDto.CompanyId);
                if (company == null)
                    return ServiceResult<TripDto>.Failure($"Company with ID {createDto.CompanyId} not found.");

                // Validate driver exists - using the Users repository that expects a string ID
                var driver = await _unitOfWork.Users.GetDriverByUserIdAsync(createDto.DriverId);
                if (driver == null)
                    return ServiceResult<TripDto>.Failure($"Driver with ID {createDto.DriverId} not found.");

                // Validate bus if provided
                Domain.Entities.Bus bus = null;
                if (createDto.BusId.HasValue)
                {
                    bus = await _unitOfWork.Buses.GetByIdAsync(createDto.BusId.Value);
                    if (bus == null)
                        return ServiceResult<TripDto>.Failure($"Bus with ID {createDto.BusId} not found.");
                    
                    // Check if bus is available (not assigned to another active trip)
                    var busInUse = await _unitOfWork.Trips.AnyAsync(t => 
                        t.BusId == createDto.BusId && 
                        !t.IsCompleted && 
                        !t.IsDeleted);
                    
                    if (busInUse)
                        return ServiceResult<TripDto>.Failure($"Bus with ID {createDto.BusId} is already assigned to another active trip.");
                }

                // Create new trip entity
                var trip = new Domain.Entities.Trip
                {
                    RouteId = createDto.RouteId,
                    CompanyId = createDto.CompanyId,
                    DriverId = createDto.DriverId,
                    BusId = createDto.BusId,
                    DepartureTime = createDto.DepartureTime,
                    AvailableSeats = createDto.AvailableSeats,
                    Price = createDto.Price,
                    IsCompleted = false
                };

                // Add trip
                await _unitOfWork.Trips.AddAsync(trip);
                await _unitOfWork.SaveChangesAsync();

                // Now create trip stations
                if (createDto.TripStations != null && createDto.TripStations.Any())
                {
                    var tripStations = new List<TripStation>();
                    foreach (var stationDto in createDto.TripStations.OrderBy(s => s.SequenceNumber))
                    {
                        // Validate station exists
                        var station = await _unitOfWork.Stations.GetByIdAsync(stationDto.StationId);
                        if (station == null)
                            return ServiceResult<TripDto>.Failure($"Station with ID {stationDto.StationId} not found.");

                        var tripStation = new TripStation
                        {
                            TripId = trip.Id,
                            StationId = stationDto.StationId,
                            SequenceNumber = stationDto.SequenceNumber,
                            ArrivalTime = stationDto.ArrivalTime,
                            DepartureTime = stationDto.DepartureTime,
                            IsActive = true
                        };
                        
                        tripStations.Add(tripStation);
                    }

                    // Save all trip stations at once
                    await _unitOfWork.TripStations.AddRangeAsync(tripStations);
                    await _unitOfWork.SaveChangesAsync();

                    // Set trip's arrival time based on the last station's arrival time
                    var lastStation = tripStations.OrderByDescending(ts => ts.SequenceNumber).FirstOrDefault();
                    if (lastStation != null)
                    {
                        trip.ArrivalTime = lastStation.ArrivalTime;
                        _unitOfWork.Trips.Update(trip);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                // Get updated trip with all details
                var createdTrip = await _unitOfWork.Trips.GetTripByIdWithDetailsAsync(trip.Id);
                var tripDto = MapTripToDto(createdTrip);

                return ServiceResult<TripDto>.Success(tripDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trip");
                return ServiceResult<TripDto>.Failure($"Error creating trip: {ex.Message}");
            }
        }

        public async Task<ServiceResult<TripDto>> GetTripByIdAsync(int tripId)
        {
            try
            {
                var trip = await _unitOfWork.Trips.GetTripByIdWithDetailsAsync(tripId);
                if (trip == null)
                    return ServiceResult<TripDto>.Failure($"Trip with ID {tripId} not found.");

                var tripDto = MapTripToDto(trip);
                return ServiceResult<TripDto>.Success(tripDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving trip with ID {tripId}");
                return ServiceResult<TripDto>.Failure($"Error retrieving trip: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<TripDto>>> GetAllTripsByCompanyAsync(int companyId)
        {
            try
            {
                var trips = await _unitOfWork.Trips.GetTripsByCompanyIdAsync(companyId);
                var tripsDto = trips.Select(MapTripToDto).ToList();
                
                return ServiceResult<IEnumerable<TripDto>>.Success(tripsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving trips for company ID {companyId}");
                return ServiceResult<IEnumerable<TripDto>>.Failure($"Error retrieving trips: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PaginatedTripDto>> GetPaginatedTripsAsync(int companyId, int pageNumber, int pageSize)
        {
            try
            {
                var paginatedResult = await _unitOfWork.Trips.GetPaginatedTripsByCompanyIdAsync(companyId, pageNumber, pageSize);
                
                var paginatedDto = new PaginatedTripDto
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = paginatedResult.TotalCount,
                    TotalPages = (int)Math.Ceiling(paginatedResult.TotalCount / (double)pageSize),
                    Trips = paginatedResult.Trips.Select(MapTripToDto).ToList()
                };

                return ServiceResult<PaginatedTripDto>.Success(paginatedDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving paginated trips for company ID {companyId}");
                return ServiceResult<PaginatedTripDto>.Failure($"Error retrieving paginated trips: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<TripDto>>> GetTripsByRouteAsync(int routeId)
        {
            try
            {
                var trips = await _unitOfWork.Trips.GetTripsByRouteIdAsync(routeId);
                var tripsDto = trips.Select(MapTripToDto).ToList();
                
                return ServiceResult<IEnumerable<TripDto>>.Success(tripsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving trips for route ID {routeId}");
                return ServiceResult<IEnumerable<TripDto>>.Failure($"Error retrieving trips: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<TripDto>>> GetTripsByDriverAsync(string driverId)
        {
            try
            {
                var trips = await _unitOfWork.Trips.GetTripsByDriverIdAsync(driverId);
                var tripsDto = trips.Select(MapTripToDto).ToList();
                
                return ServiceResult<IEnumerable<TripDto>>.Success(tripsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving trips for driver ID {driverId}");
                return ServiceResult<IEnumerable<TripDto>>.Failure($"Error retrieving trips: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateTripStatusAsync(UpdateTripStatusDto updateDto)
        {
            try
            {
                var success = await _unitOfWork.Trips.UpdateTripStatusAsync(updateDto.TripId, updateDto.IsCompleted);
                if (!success)
                    return ServiceResult.Failure($"Trip with ID {updateDto.TripId} not found.");

                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for trip ID {updateDto.TripId}");
                return ServiceResult.Failure($"Error updating trip status: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteTripAsync(int tripId)
        {
            try
            {
                var trip = await _unitOfWork.Trips.GetByIdAsync(tripId);
                if (trip == null)
                    return ServiceResult.Failure($"Trip with ID {tripId} not found.");

                // Check if the trip has associated bookings
                // Since there's no direct access to Bookings repository in the UnitOfWork,
                // we'll need to use a different approach - soft deletion instead
                
                trip.IsDeleted = true;
                _unitOfWork.Trips.Update(trip);
                await _unitOfWork.SaveChangesAsync();
                
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting trip ID {tripId}");
                return ServiceResult.Failure($"Error deleting trip: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<TripDto>>> GetAllFutureTripsAsync()
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                var trips = await _unitOfWork.Trips.GetAllFutureTripsAsync(currentDate);
                var tripsDto = trips.Select(MapTripToDto).ToList();
                
                return ServiceResult<IEnumerable<TripDto>>.Success(tripsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving future trips");
                return ServiceResult<IEnumerable<TripDto>>.Failure($"Error retrieving future trips: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<TripDto>>> SearchTripsAsync(int startCityId, int endCityId, DateTime departureDate, int requiredSeats)
        {
            try
            {
                // Validate that the cities exist
                var startCity = await _unitOfWork.Cities.GetByIdAsync(startCityId);
                if (startCity == null)
                    return ServiceResult<IEnumerable<TripDto>>.Failure($"Start city with ID {startCityId} not found.");
                
                var endCity = await _unitOfWork.Cities.GetByIdAsync(endCityId);
                if (endCity == null)
                    return ServiceResult<IEnumerable<TripDto>>.Failure($"End city with ID {endCityId} not found.");

                if (requiredSeats <= 0)
                    return ServiceResult<IEnumerable<TripDto>>.Failure("Required seats must be greater than 0.");

                var trips = await _unitOfWork.Trips.SearchTripsAsync(startCityId, endCityId, departureDate, requiredSeats);
                var tripsDto = trips.Select(MapTripToDto).ToList();
                
                return ServiceResult<IEnumerable<TripDto>>.Success(tripsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for trips");
                return ServiceResult<IEnumerable<TripDto>>.Failure($"Error searching for trips: {ex.Message}");
            }
        }

        private TripDto MapTripToDto(Domain.Entities.Trip trip)
        {
            if (trip == null) return null;

            var tripDto = new TripDto
            {
                Id = trip.Id,
                RouteId = trip.RouteId,
                RouteName = trip.Route?.Name ?? string.Empty,
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                DriverId = trip.DriverId,
                DriverName = trip.Driver?.AppUser?.FullName ?? string.Empty,
                DriverPhoneNumber = trip.Driver?.AppUser?.PhoneNumber ?? string.Empty,
                BusId = trip.BusId,
                BusRegistrationNumber = trip.Bus?.RegistrationNumber ?? string.Empty,
                AmenityDescription = trip.Bus?.AmenityDescription ?? string.Empty,
                IsCompleted = trip.IsCompleted,
                AvailableSeats = trip.AvailableSeats,
                CompanyId = trip.CompanyId,
                CompanyName = trip.Company?.Name ?? string.Empty,
                Price = trip.Price,
                TripStations = trip.TripStations?
                    .OrderBy(ts => ts.SequenceNumber)
                    .Select(ts => new TripStationResponseDto
                    {
                        StationId = ts.StationId,
                        StationName = ts.Station?.Name ?? string.Empty,
                        CityId = ts.Station?.CityId ?? 0,
                        CityName = ts.Station?.City?.Name ?? string.Empty,
                        SequenceNumber = ts.SequenceNumber,
                        ArrivalTime = ts.ArrivalTime,
                        DepartureTime = ts.DepartureTime
                    })
                    .ToList() ?? new List<TripStationResponseDto>()
            };

            return tripDto;
        }
    }
}