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
        Task AddStation(StationDto stationDto);

        Task AddStationsAsync(List<StationDto> stationDtos);
        Task<bool> UpdateStation(int id, StationDto stationDto);
        Task<bool> DeleteStation(int id);
    }
}
