using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface ICityRepository : IGenericRepository<City>
    {
        Task<IEnumerable<City>> GetCitiesByGovernorateAsync(string governorate);
        Task<IEnumerable<City>> AddCitiesAndReturnThemAsync(IEnumerable<City> cities);
    }
}