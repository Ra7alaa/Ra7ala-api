using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IStationRepository : IGenericRepository<Station>
    {
        Task<IEnumerable<Station>> GetStationsByCityIdAsync(int cityId);
        Task<IEnumerable<Station>> GetStationsByCompanyIdAsync(int companyId);
        Task<IEnumerable<Station>> GetStationsByCityNameAsync(string cityName);
        Task<IEnumerable<Station>> GetNearbyStationsAsync(double latitude, double longitude, double radiusInKm);
        
        // الوظائف الجديدة للتمييز بين أنواع المحطات
        Task<IEnumerable<Station>> GetSystemStationsAsync();
        Task<IEnumerable<Station>> GetCompanyStationsAsync(int companyId);
        
        // دالة جديدة للحصول على محطات النظام مع محطات شركة محددة
        Task<IEnumerable<Station>> GetSystemAndCompanyStationsAsync(int companyId);
    }
}