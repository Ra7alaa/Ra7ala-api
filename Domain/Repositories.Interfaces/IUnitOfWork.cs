using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
         // Generic Repositories
        IGenericRepository<City> Cities { get; }
        IGenericRepository<Station> Stations { get; }
        IGenericRepository<Company> Companies { get; }
       // IGenericRepository<CompanyRating> CompanyRatings { get; }

        // Custom Repositories
        ICompanyRepository CompanyRepository { get; }

        // Save changes
        Task<int> SaveChangesAsync();
    }
}