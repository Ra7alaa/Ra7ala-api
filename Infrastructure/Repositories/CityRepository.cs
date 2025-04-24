using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        public CityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<City>> GetCitiesByGovernorateAsync(string governorate)
        {
            return await _context.Cities
                .Where(c => c.Governorate == governorate && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<City>> AddCitiesAndReturnThemAsync(IEnumerable<City> cities)
        {
            await _dbSet.AddRangeAsync(cities);
            return cities;
        }

        // تجاوز الطرق الأساسية للتأكد من عدم إرجاع المدن المحذوفة
        public new async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _context.Cities
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public new async Task<City?> GetByIdAsync(int id)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}