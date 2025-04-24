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
        Task<CityDto> AddCityAsync(CityAddUpdateDto cityDto);
        Task<List<CityDto>> AddCitiesAsync(List<CityAddUpdateDto> cities);
        Task<CityDto?> UpdateCityAsync(int id, CityAddUpdateDto cityDto);
        Task<bool> DeleteCityAsync(int id);
        Task<IEnumerable<CityDto>> GetCitiesByGovernorateAsync(string governorate);
        
        // طرق جديدة للحصول على المدن مع المحطات
        Task<IEnumerable<CityDto>> GetAllCitiesWithStationsAsync();
        Task<CityDto?> GetCityWithStationsByIdAsync(int id);
    }
}
