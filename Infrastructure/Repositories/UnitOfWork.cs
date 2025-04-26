using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories.Company;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private ICityRepository? _citiesRepository;
        private IStationRepository? _stationsRepository;
        private IUserRepository? _userRepository;


        public UnitOfWork(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public ICompanyRepository CompanyRepository => 
            _companyRepository ??= new CompanyRepository(_context);

        public ICityRepository Cities => 
            _citiesRepository ??= new CityRepository(_context);

        public IStationRepository Stations => 
            _stationsRepository ??= new StationRepository(_context);
 
        public IUserRepository Users => 
            _userRepository ??= new UserRepository(_context, _userManager, _signInManager);

        public void Add<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
        }

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