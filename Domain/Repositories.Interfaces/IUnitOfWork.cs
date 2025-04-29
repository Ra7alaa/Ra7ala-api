using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IUnitOfWork
    {

        // Custom Repositories
        ICompanyRepository CompanyRepository { get; }
        ICityRepository Cities { get; }
        IStationRepository Stations { get; } 
        IUserRepository Users { get; }
        IRouteRepository Routes { get; }

        // Generic Repositories
        IGenericRepository<RouteStation> RouteStations { get; }
        IGenericRepository<SuperAdmin> SuperAdmins { get; }
        IGenericRepository<Admin> Admins { get; }
        IGenericRepository<Driver> Drivers { get; }
        IGenericRepository<Passenger> Passengers { get; }


        // Save changes
        Task<int> SaveChangesAsync();
    }
}