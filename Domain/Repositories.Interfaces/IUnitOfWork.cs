using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<City> Cities { get; }
        IGenericRepository<Station> Stations { get; }
        IGenericRepository<Company> Companies { get; }
        
        Task<int> SaveChangesAsync();
    }
}