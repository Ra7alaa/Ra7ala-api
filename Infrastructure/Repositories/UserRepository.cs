using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Data;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserRepository(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> IsSuperAdminExistsForCompanyAsync(int companyId)
        {
            return await _context.Set<SuperAdmin>()
                .AnyAsync(sa => sa.CompanyId == companyId);
        }

        public Task<string?> GetUserIdFromClaimsPrincipalAsync(ClaimsPrincipal user)
        {
            // Since _userManager.GetUserId is not an async method, we wrap it in a Task.FromResult
            return Task.FromResult(_userManager.GetUserId(user));
        }

        public async Task<SuperAdmin?> GetSuperAdminByUserIdAsync(string userId)
        {
            return await _context.Set<SuperAdmin>()
                .Include(sa => sa.Company)
                .FirstOrDefaultAsync(sa => sa.Id == userId);
        }

        public async Task<Admin?> GetAdminByUserIdAsync(string userId)
        {
            return await _context.Set<Admin>()
                .Include(a => a.Company)
                .FirstOrDefaultAsync(a => a.Id == userId);
        }

        public async Task<Driver?> GetDriverByUserIdAsync(string userId)
        {
            return await _context.Set<Driver>()
                .Include(d => d.Company)
                .FirstOrDefaultAsync(d => d.Id == userId);
        }

        public async Task<Passenger?> GetPassengerByUserIdAsync(string userId)
        {
            return await _context.Set<Passenger>()
                .FirstOrDefaultAsync(p => p.Id == userId);
        }

        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckPasswordAsync(AppUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<AppUser?> GetSystemOwnerAsync()
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserType == UserType.Owner);
        }

        public async Task<IEnumerable<Admin>> GetAdminsByCompanyIdAsync(int companyId)
        {
            return await _context.Set<Admin>()
                .Include(a => a.AppUser)
                .Include(a => a.Company)
                .Where(a => a.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Driver>> GetDriversByCompanyIdAsync(int companyId)
        {
            return await _context.Set<Driver>()
                .Include(d => d.AppUser)
                .Include(d => d.Company)
                .Where(d => d.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<bool> UserHasAccessToCompanyAsync(string userId, int companyId)
        {
            // Check if user is null
            if (string.IsNullOrEmpty(userId))
                return false;

            // First check if user is SuperAdmin for this company
            var superAdmin = await GetSuperAdminByUserIdAsync(userId);
            if (superAdmin != null && superAdmin.CompanyId == companyId)
                return true;

            // Then check if user is Admin for this company
            var admin = await GetAdminByUserIdAsync(userId);
            if (admin != null && admin.CompanyId == companyId)
                return true;

            // User doesn't have access to this company
            return false;
        }

        public async Task<bool> UserHasAccessToCompanyAsync(ClaimsPrincipal user, int companyId)
        {
            if (user == null)
                return false;

            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return false;

            return await UserHasAccessToCompanyAsync(userId, companyId);
        }
    }
}