using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IRouteRepository : IGenericRepository<Route>
    {
        Task<Route> GetRouteByIdWithDetailsAsync(int routeId);
        Task<List<Route>> GetRoutesByCompanyIdAsync(int companyId);
        Task<PaginatedRoutesResult> GetPaginatedRoutesByCompanyIdAsync(int companyId, int pageNumber, int pageSize);
        Task<bool> IsStationInCityAsync(int stationId, int cityId);
    }

    public class PaginatedRoutesResult
    {
        public int TotalCount { get; set; }
        public List<Route> Routes { get; set; } = new List<Route>();
    }
}