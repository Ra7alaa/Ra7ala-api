using Application.DTOs;
using Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IRouteService
    {
        Task<ServiceResult<RouteDto>> CreateRouteAsync(CreateRouteDto createDto);
        Task<ServiceResult<PaginatedRouteDto>> GetPaginatedRoutesAsync(int companyId, int pageNumber, int pageSize);
        Task<ServiceResult<IEnumerable<RouteDto>>> GetAllRoutesAsync(int companyId);
        Task<ServiceResult<RouteDto>> GetRouteByIdAsync(int routeId);
        Task<ServiceResult<RouteDto>> UpdateRouteAsync(UpdateRouteDto updateDto);
        Task<ServiceResult> DeleteRouteAsync(int routeId);
    }
}