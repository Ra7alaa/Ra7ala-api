using Domain.Entities;
using System.Security.Claims;

namespace Domain.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> IsSuperAdminExistsForCompanyAsync(int companyId);
        Task<string?> GetUserIdFromClaimsPrincipalAsync(ClaimsPrincipal user);
        Task<SuperAdmin?> GetSuperAdminByUserIdAsync(string userId);
        Task<Admin?> GetAdminByUserIdAsync(string userId);
        Task<Driver?> GetDriverByUserIdAsync(string userId);
        Task<Passenger?> GetPassengerByUserIdAsync(string userId);
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(AppUser user, string password);
    }
}