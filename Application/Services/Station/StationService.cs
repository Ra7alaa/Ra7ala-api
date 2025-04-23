using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.City;
using Application.DTOs.Station;
using Application.Map;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace Application.Services.Station
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddStation(StationDto stationDto)
        {
            var station = stationDto.ToEntity();
            await _unitOfWork.Stations.AddAsync(station);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteStation(int id)
        {
            var station = await _unitOfWork.Stations.GetByIdAsync(id);

            if (station == null)
            {
                return false;
            }

            station.IsDeleted = true;
            _unitOfWork.Stations.Update(station);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<StationDto?> GetStationById(int id)
        {
            var station = await _unitOfWork.Stations.GetByIdAsync(id);
            return station?.ToDto();
        }

        public async Task<IEnumerable<StationDto>> GetStations()
        {
            var stations = await _unitOfWork.Stations.GetAllAsync();
            return stations.ToDtoList();
        }

        public async Task<IEnumerable<StationDto>> GetStationsByCityName(string cityName)
        {
            var stations = await _unitOfWork.Stations.GetStationsByCityNameAsync(cityName);
            return stations.ToDtoList();
        }

        public async Task<IEnumerable<StationDto>> GetStationsByCityId(int cityId)
        {
            var stations = await _unitOfWork.Stations.GetStationsByCityIdAsync(cityId);
            return stations.ToDtoList();
        }

        public async Task<IEnumerable<StationDto>> GetStationsByCompanyId(int companyId)
        {
            var stations = await _unitOfWork.Stations.GetStationsByCompanyIdAsync(companyId);
            return stations.ToDtoList();
        }

        public async Task<IEnumerable<StationDto>> GetNearbyStations(double latitude, double longitude, double radiusInKm)
        {
            var stations = await _unitOfWork.Stations.GetNearbyStationsAsync(latitude, longitude, radiusInKm);
            return stations.ToDtoList();
        }

        public async Task<bool> UpdateStation(int id, StationDto stationDto)
        {
            var station = await _unitOfWork.Stations.GetByIdAsync(id);

            if (station == null)
            {
                return false;
            }

            stationDto.ToEntity(station); // يقوم بتحديث الكائن الموجود
            _unitOfWork.Stations.Update(station);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task AddStationsAsync(List<StationDto> stationDtos)
        {
            var stations = new List<Domain.Entities.Station>();
            foreach (var stationDto in stationDtos)
            {
                var station = stationDto.ToEntity();
                stations.Add(station);
            }

           await _unitOfWork.Stations.AddRangeAsync(stations);
            await _unitOfWork.SaveChangesAsync();
            
        }
    }
}

