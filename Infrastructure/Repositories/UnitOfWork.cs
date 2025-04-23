using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<City> _citiesRepository;
        private IStationRepository _stationsRepository;
        private IGenericRepository<Company> _companiesRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<City> Cities => 
            _citiesRepository ??= new GenericRepository<City>(_context);

        public IStationRepository Stations => 
            _stationsRepository ??= new StationRepository(_context);

        public IGenericRepository<Company> Companies => 
            _companiesRepository ??= new GenericRepository<Company>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}