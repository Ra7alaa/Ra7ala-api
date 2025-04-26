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
        
        void Add<T>(T entity) where T : class;

        // Save changes
        Task<int> SaveChangesAsync();
    }
}