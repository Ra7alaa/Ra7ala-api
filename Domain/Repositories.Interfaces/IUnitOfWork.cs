using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        ICityRepository Cities { get; }
        IStationRepository Stations { get; } // تم تغييرها من IGenericRepository<Station> إلى IStationRepository
        IGenericRepository<Company> Companies { get; }
        
        Task<int> SaveChangesAsync();
    }
}