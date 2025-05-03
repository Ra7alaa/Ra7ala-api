// Domain.Repositories.Interfaces/IUnitOfWork.cs
using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IUnitOfWork
    {   
        // Custom Repositories
        IBusRepository Buses { get; }
        ICompanyRepository CompanyRepository { get; }
        ICityRepository Cities { get; }
        IStationRepository Stations { get; }
        IUserRepository Users { get; }
        IRouteRepository Routes { get; }
        ITripRepository Trips { get; }
        IBookingRepository Bookings { get; }
        ITicketRepository Tickets { get; }

        // Generic Repositories
        IGenericRepository<RouteStation> RouteStations { get; }
        IGenericRepository<SuperAdmin> SuperAdmins { get; }
        IGenericRepository<Admin> Admins { get; }
        IGenericRepository<Driver> Drivers { get; }
        IGenericRepository<Passenger> Passengers { get; }
        IGenericRepository<TripStation> TripStations { get; }

        Task<int> SaveChangesAsync();
    }
}