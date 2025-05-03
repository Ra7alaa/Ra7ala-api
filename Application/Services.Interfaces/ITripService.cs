using Application.DTOs.Trip;
using Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ITripService
    {
        Task<ServiceResult<TripDto>> CreateTripAsync(CreateTripDto createDto);
        Task<ServiceResult<PaginatedTripDto>> GetPaginatedTripsAsync(int companyId, int pageNumber, int pageSize);
        Task<ServiceResult<IEnumerable<TripDto>>> GetAllTripsByCompanyAsync(int companyId);
        Task<ServiceResult<IEnumerable<TripDto>>> GetTripsByRouteAsync(int routeId);
        Task<ServiceResult<IEnumerable<TripDto>>> GetTripsByDriverAsync(string driverId);
        Task<ServiceResult<TripDto>> GetTripByIdAsync(int tripId);
        Task<ServiceResult> UpdateTripStatusAsync(UpdateTripStatusDto updateDto);
        Task<ServiceResult> DeleteTripAsync(int tripId);
        
        // New methods
        Task<ServiceResult<IEnumerable<TripDto>>> GetAllFutureTripsAsync();
        Task<ServiceResult<IEnumerable<TripDto>>> SearchTripsAsync(int startCityId, int endCityId, DateTime departureDate, int requiredSeats);
    }
}