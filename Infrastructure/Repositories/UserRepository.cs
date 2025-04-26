using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
    }
}