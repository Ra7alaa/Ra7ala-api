using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RouteRepository : GenericRepository<Route>, IRouteRepository
    {
        public RouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Route> GetRouteByIdWithDetailsAsync(int routeId)
        {
            return await _dbSet
                .Include(r => r.Company)
                .Include(r => r.StartCity)
                .Include(r => r.EndCity)
                .Include(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                        .ThenInclude(s => s.City)
                .FirstOrDefaultAsync(r => r.Id == routeId && !r.IsDeleted);
        }

        public async Task<List<Route>> GetRoutesByCompanyIdAsync(int companyId)
        {
            return await _dbSet
                .Include(r => r.Company)
                .Include(r => r.StartCity)
                .Include(r => r.EndCity)
                .Include(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                        .ThenInclude(s => s.City)
                .Where(r => r.CompanyId == companyId && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<PaginatedRoutesResult> GetPaginatedRoutesByCompanyIdAsync(int companyId, int pageNumber, int pageSize)
        {
            var query = _dbSet
                .Include(r => r.Company)
                .Include(r => r.StartCity)
                .Include(r => r.EndCity)
                .Include(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
                        .ThenInclude(s => s.City)
                .Where(r => r.CompanyId == companyId && !r.IsDeleted);

            var totalCount = await query.CountAsync();
            var routes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedRoutesResult
            {
                TotalCount = totalCount,
                Routes = routes
            };
        }

        public async Task<bool> IsStationInCityAsync(int stationId, int cityId)
        {
            var station = await _context.Stations
                .Where(s => s.Id == stationId && !s.IsDeleted)
                .Select(s => new { s.CityId })
                .FirstOrDefaultAsync();

            return station != null && station.CityId == cityId;
        }
    }
}