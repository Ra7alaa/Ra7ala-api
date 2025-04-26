using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        // Custom Repositories
        ICompanyRepository CompanyRepository { get; }

        // Save changes
        Task<int> SaveChangesAsync();
    }
}