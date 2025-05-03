using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories.Company;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IBusRepository? _busRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private ICompanyRepository? _companyRepository; // Changed to nullable
        private ICityRepository? _citiesRepository;
        private IStationRepository? _stationsRepository;
        private IUserRepository? _userRepository;
        private IRouteRepository? _routeRepository;
        private ITripRepository? _tripRepository;
        private IGenericRepository<RouteStation>? _routeStationsRepository;
        private IGenericRepository<SuperAdmin>? _superAdminsRepository;
        private IGenericRepository<Admin>? _adminsRepository;
        private IGenericRepository<Driver>? _driversRepository;
        private IGenericRepository<Passenger>? _passengersRepository;
        private IGenericRepository<TripStation>? _tripStationsRepository;

        public UnitOfWork(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Custom Repositories
        public ICompanyRepository CompanyRepository => 
            _companyRepository ??= new CompanyRepository(_context);
        public IBusRepository Buses => 
            _busRepository ??= new BusRepository(_context);

        public ICityRepository Cities => 
            _citiesRepository ??= new CityRepository(_context);

        public IStationRepository Stations => 
            _stationsRepository ??= new StationRepository(_context);
 
        public IUserRepository Users => 
            _userRepository ??= new UserRepository(_context, _userManager, _signInManager);

        public IRouteRepository Routes => 
            _routeRepository ??= new RouteRepository(_context);

        public ITripRepository Trips => 
            _tripRepository ??= new TripRepository(_context);
        
        #endregion

        #region Generic Repositories
        public IGenericRepository<RouteStation> RouteStations => 
            _routeStationsRepository ??= new GenericRepository<RouteStation>(_context);

        public IGenericRepository<SuperAdmin> SuperAdmins => 
            _superAdminsRepository ??= new GenericRepository<SuperAdmin>(_context);

        public IGenericRepository<Admin> Admins => 
            _adminsRepository ??= new GenericRepository<Admin>(_context);

        public IGenericRepository<Driver> Drivers => 
            _driversRepository ??= new GenericRepository<Driver>(_context);

        public IGenericRepository<Passenger> Passengers => 
            _passengersRepository ??= new GenericRepository<Passenger>(_context); 

        public IGenericRepository<TripStation> TripStations =>
            _tripStationsRepository ??= new GenericRepository<TripStation>(_context);    

        #endregion
        
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}