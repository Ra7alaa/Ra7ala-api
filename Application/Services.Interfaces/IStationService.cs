using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Station;

namespace Application.Services.Interfaces
{
    public interface IStationService
    {
        Task<StationDto?> GetStationById(int id);
        Task<IEnumerable<StationDto>> GetStations();
        Task<IEnumerable<StationDto>> GetStationsByCityName(string cityName);
        Task<IEnumerable<StationDto>> GetStationsByCityId(int cityId);
        Task<IEnumerable<StationDto>> GetStationsByCompanyId(int companyId);
        Task<IEnumerable<StationDto>> GetNearbyStations(double latitude, double longitude, double radiusInKm);
        
        // الوظائف المعدلة للتمييز بين أنواع المحطات
        Task<IEnumerable<StationDto>> GetSystemStations();
        Task<IEnumerable<StationDto>> GetCompanyStations(int companyId);
        Task<IEnumerable<StationDto>> GetSystemAndCompanyStations(int companyId);
        
        // دوال لإضافة محطات النظام (SystemStations)
        Task<StationDto> AddStation(StationAddUpdateDto stationDto);
        Task<List<StationDto>> AddStationsAsync(List<StationAddUpdateDto> stationDtos);
        
        // دوال جديدة لإضافة محطات الشركات (CompanyStations)
        Task<StationDto> AddCompanyStation(CompanyStationAddUpdateDto companyStationDto);
        Task<List<StationDto>> AddCompanyStationsAsync(List<CompanyStationAddUpdateDto> companyStationDtos);
        
        // دوال تحديث المحطات
        Task<StationDto?> UpdateStation(int id, StationAddUpdateDto stationDto);
        Task<StationDto?> UpdateCompanyStation(int id, CompanyStationAddUpdateDto companyStationDto);
        
        // دوال الحذف المعدلة
        Task<bool> DeleteStation(int id);
        Task<bool> DeleteSystemStation(int id); // حذف محطة نظام
        Task<bool> DeleteCompanyStation(int id, int companyId); // حذف محطة شركة
    }
}
