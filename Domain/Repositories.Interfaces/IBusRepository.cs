using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IBusRepository : IGenericRepository<Bus>
    {
        Task<IEnumerable<Bus>> GetBusesByCompanyIdAsync(int companyId);
    }
}