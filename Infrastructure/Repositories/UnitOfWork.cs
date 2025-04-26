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
        private ICompanyRepository _companyRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Custom Repositories
        public ICompanyRepository CompanyRepository => 
            _companyRepository ??= new CompanyRepository(_context);
            
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