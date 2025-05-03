using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TripRepository : GenericRepository<Trip>, ITripRepository
    {
        public TripRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Trip> GetTripByIdWithDetailsAsync(int tripId)
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.AppUser)
                .Include(t => t.Company)
                .Include(t => t.Bus)
                .Include(t => t.TripStations)
                    .ThenInclude(ts => ts.Station)
                        .ThenInclude(s => s.City)
                .FirstOrDefaultAsync(t => t.Id == tripId && !t.IsDeleted);
        }

        public async Task<List<Trip>> GetTripsByCompanyIdAsync(int companyId)
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.AppUser)
                .Include(t => t.Company)
                .Include(t => t.Bus)
                .Include(t => t.TripStations)
                    .ThenInclude(ts => ts.Station)
                .Where(t => t.CompanyId == companyId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetTripsByRouteIdAsync(int routeId)
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.AppUser)
                .Include(t => t.Company)
                .Include(t => t.Bus)
                .Include(t => t.TripStations)
                    .ThenInclude(ts => ts.Station)
                .Where(t => t.RouteId == routeId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetTripsByDriverIdAsync(string driverId)
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.AppUser)
                .Include(t => t.Company)
                .Include(t => t.Bus)
                .Include(t => t.TripStations)
                    .ThenInclude(ts => ts.Station)
                .Where(t => t.DriverId == driverId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<PaginatedTripsResult> GetPaginatedTripsByCompanyIdAsync(int companyId, int pageNumber, int pageSize)
        {
            var query = _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.AppUser)
                .Include(t => t.Company)
                .Include(t => t.Bus)
                .Include(t => t.TripStations)
                    .ThenInclude(ts => ts.Station)
                .Where(t => t.CompanyId == companyId && !t.IsDeleted);

            var totalCount = await query.CountAsync();
            var trips = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedTripsResult
            {
                TotalCount = totalCount,
                Trips = trips
            };
        }

        public async Task<bool> UpdateTripStatusAsync(int tripId, bool isCompleted)
        {
            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null || trip.IsDeleted)
                return false;

            trip.IsCompleted = isCompleted;
            _context.Trips.Update(trip);
            return true;
        }

        public async Task<bool> AnyAsync(Expression<Func<Trip, bool>> predicate)
        {
            return await _context.Trips.AnyAsync(predicate);
        }

        // Use new keyword for overriding non-virtual methods from GenericRepository
        public new async Task<IEnumerable<Trip>> GetAllAsync()
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.AppUser)
                .Include(t => t.Company)
                .Include(t => t.Bus)
                .Where(t => !t.IsDeleted)
                .ToListAsync();
        }

        public new async Task<Trip> GetByIdAsync(int id)
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Driver)
                .Include(t => t.Company)
                .Include(t => t.Bus)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        // Add Update method that can be used synchronously
        public new void Update(Trip entity)
        {
            _context.Trips.Update(entity);
        }

        public async Task<List<Trip>> GetAllFutureTripsAsync(DateTime currentDate)
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.AppUser)
                .Include(t => t.Company)
                .Include(t => t.Bus)
                .Include(t => t.TripStations)
                    .ThenInclude(ts => ts.Station)
                        .ThenInclude(s => s.City)
                .Where(t => t.DepartureTime > currentDate && !t.IsDeleted && !t.IsCompleted)
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();
        }

        public async Task<List<Trip>> SearchTripsAsync(int startCityId, int endCityId, DateTime departureDate, int requiredSeats)
        {
            // Get trips that have stations in both the start and end cities, have available seats, and depart on the specified date
            var startOfDay = departureDate.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Driver)
                    .ThenInclude(d => d.AppUser)
                .Include(t => t.Company)
                .Include(t => t.Bus)
                .Include(t => t.TripStations)
                    .ThenInclude(ts => ts.Station)
                        .ThenInclude(s => s.City)
                .Where(t => 
                    t.DepartureTime >= startOfDay && 
                    t.DepartureTime <= endOfDay &&
                    t.AvailableSeats >= requiredSeats &&
                    !t.IsDeleted &&
                    !t.IsCompleted &&
                    t.TripStations.Any(ts => ts.Station.CityId == startCityId) &&
                    t.TripStations.Any(ts => ts.Station.CityId == endCityId))
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();
        }
    }
}