using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories.Company;
using System;
using System.Threading.Tasks;
using Company = Domain.Entities.Company;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed;

        // Generic repositories
        private IGenericRepository<City> _citiesRepository;
        private IGenericRepository<Station> _stationsRepository;
        private IGenericRepository<Domain.Entities.Company> _companiesRepository;
        private ICompanyRepository _companyRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IGenericRepository<City> Cities => 
            _citiesRepository ??= new GenericRepository<City>(_context);

        public IGenericRepository<Station> Stations => 
            _stationsRepository ??= new GenericRepository<Station>(_context);

        public IGenericRepository<Domain.Entities.Company> Companies => 
            _companiesRepository ??= new GenericRepository<Domain.Entities.Company>(_context);

        public ICompanyRepository CompanyRepository => 
            _companyRepository ??= new CompanyRepository(_context);

        IGenericRepository<Domain.Entities.Company> IUnitOfWork.Companies => throw new NotImplementedException();

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}