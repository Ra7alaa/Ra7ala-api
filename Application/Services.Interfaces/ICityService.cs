using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.City;

namespace Application.Services.Interfaces
{
    public interface ICityService
    {
        Task<IEnumerable<CityDto>> GetAllCitiesAsync();
        Task<CityDto?> GetCityByIdAsync(int id);
        Task AddCityAsync(CityDto cityDto);

        Task AddCitiesAsync(List<CityDto> cities);
        Task<bool> UpdateCityAsync(int id,CityDto cityDto);
        Task<bool> DeleteCityAsync(int id);
    }
}
