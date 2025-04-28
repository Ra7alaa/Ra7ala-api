using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BusRepository : GenericRepository<Bus>, IBusRepository
    {
        public BusRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Override GetAllAsync to exclude deleted buses
        public new async Task<IEnumerable<Bus>> GetAllAsync()
        {
            return await _context.Buses
                .Include(b => b.Company)
                .Where(b => !b.IsDeleted)
                .ToListAsync();

        }

        // Override GetByIdAsync to exclude deleted buses
     public new async Task<Bus> GetByIdAsync(int id)
{
    return await _context.Buses
        .Include(b => b.Company)
        .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted) 
        ?? throw new KeyNotFoundException($"Bus with id {id} not found");
}

        public async Task<IEnumerable<Bus>> GetBusesByCompanyIdAsync(int companyId)
        {
            return await _context.Buses
                .Include(b => b.Company)
                .Where(b => b.CompanyId == companyId && !b.IsDeleted)
                .ToListAsync();
        }
    }
}