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

        public async Task<StationDto> AddStation(StationAddUpdateDto stationDto)
        {
            var station = stationDto.ToEntity();
            await _unitOfWork.Stations.AddAsync(station);
            await _unitOfWork.SaveChangesAsync();
            
            // Return a complete DTO with the generated ID
            return station.ToDto();
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

        public async Task<StationDto?> UpdateStation(int id, StationAddUpdateDto stationDto)
        {
            var station = await _unitOfWork.Stations.GetByIdAsync(id);

            if (station == null)
            {
                return null;
            }

            stationDto.ToEntity(station); // Updates the existing entity
            _unitOfWork.Stations.Update(station);
            await _unitOfWork.SaveChangesAsync();

            // Return a complete DTO after the update
            return station.ToDto();
        }

        public async Task<List<StationDto>> AddStationsAsync(List<StationAddUpdateDto> stationDtos)
        {
            var stations = stationDtos.Select(dto => dto.ToEntity()).ToList();
            
            await _unitOfWork.Stations.AddRangeAsync(stations);
            await _unitOfWork.SaveChangesAsync();
            
            // Return a list of complete DTOs with new IDs
            return stations.Select(s => s.ToDto()).ToList();
        }
        
        // Implementing functions to distinguish between station types
        public async Task<IEnumerable<StationDto>> GetSystemStations()
        {
            var stations = await _unitOfWork.Stations.GetSystemStationsAsync();
            return stations.ToDtoList();
        }
        
        public async Task<IEnumerable<StationDto>> GetCompanyStations(int companyId)
        {
            var stations = await _unitOfWork.Stations.GetCompanyStationsAsync(companyId);
            return stations.ToDtoList();
        }
        
        // Implementation of the new function to get system stations and company stations
        public async Task<IEnumerable<StationDto>> GetSystemAndCompanyStations(int companyId)
        {
            var stations = await _unitOfWork.Stations.GetSystemAndCompanyStationsAsync(companyId);
            return stations.ToDtoList();
        }
        
        // New function to add a company station
        public async Task<StationDto> AddCompanyStation(CompanyStationAddUpdateDto companyStationDto)
        {
            // Company stations always have non-null CompanyId
            var station = companyStationDto.ToEntity();
            await _unitOfWork.Stations.AddAsync(station);
            await _unitOfWork.SaveChangesAsync();
            
            return station.ToDto();
        }
        
        // New function to add a list of company stations
        public async Task<List<StationDto>> AddCompanyStationsAsync(List<CompanyStationAddUpdateDto> companyStationDtos)
        {
            var stations = companyStationDtos.Select(dto => dto.ToEntity()).ToList();
            
            await _unitOfWork.Stations.AddRangeAsync(stations);
            await _unitOfWork.SaveChangesAsync();
            
            return stations.Select(s => s.ToDto()).ToList();
        }
        
        // Update function for a company station
        public async Task<StationDto?> UpdateCompanyStation(int id, CompanyStationAddUpdateDto companyStationDto)
        {
            var station = await _unitOfWork.Stations.GetByIdAsync(id);

            if (station == null)
            {
                return null;
            }
            
            // Ensure the station is indeed a company station
            if (station.CompanyId == null)
            {
                throw new InvalidOperationException("Cannot update a system station using a company station model");
            }

            companyStationDto.ToEntity(station); // Updates the existing entity
            _unitOfWork.Stations.Update(station);
            await _unitOfWork.SaveChangesAsync();

            return station.ToDto();
        }

        // Implementing function to delete a system station
        public async Task<bool> DeleteSystemStation(int id)
        {
            var station = await _unitOfWork.Stations.GetByIdAsync(id);

            if (station == null)
            {
                return false;
            }

            // Ensure the station is a system station
            if (station.CompanyId != null)
            {
                throw new InvalidOperationException("This station is not a system station");
            }

            station.IsDeleted = true;
            _unitOfWork.Stations.Update(station);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Implementing function to delete a company station
        public async Task<bool> DeleteCompanyStation(int id, int companyId)
        {
            var station = await _unitOfWork.Stations.GetByIdAsync(id);

            if (station == null)
            {
                return false;
            }

            // Ensure the station is a company station
            if (station.CompanyId == null)
            {
                throw new InvalidOperationException("This station is not a company station");
            }

            // Ensure the station belongs to the specified company
            if (station.CompanyId != companyId)
            {
                throw new UnauthorizedAccessException("You cannot delete a station belonging to another company");
            }

            station.IsDeleted = true;
            _unitOfWork.Stations.Update(station);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}

