using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface ITripRepository : IGenericRepository<Trip>
    {
        Task<Trip> GetTripByIdWithDetailsAsync(int tripId);
        Task<List<Trip>> GetTripsByCompanyIdAsync(int companyId);
        Task<List<Trip>> GetTripsByRouteIdAsync(int routeId);
        Task<List<Trip>> GetTripsByDriverIdAsync(string driverId);
        Task<PaginatedTripsResult> GetPaginatedTripsByCompanyIdAsync(int companyId, int pageNumber, int pageSize);
        Task<bool> UpdateTripStatusAsync(int tripId, bool isCompleted);
        Task<bool> AnyAsync(Expression<Func<Trip, bool>> predicate);
        
        // New methods for getting all future trips and searching trips
        Task<List<Trip>> GetAllFutureTripsAsync(DateTime currentDate);
        Task<List<Trip>> SearchTripsAsync(int startCityId, int endCityId, DateTime departureDate, int requiredSeats);
    }

    public class PaginatedTripsResult
    {
        public int TotalCount { get; set; }
        public List<Trip> Trips { get; set; } = new List<Trip>();
    }
}