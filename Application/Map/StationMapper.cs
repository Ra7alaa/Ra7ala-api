using Application.DTOs.Station;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Map
{
    public static class StationMapper
    {
        public static StationDto ToDto(this Station station)
        {
            if (station == null)
                return null;

            return new StationDto
            {
                Id = station.Id,
                Name = station.Name,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                CityId = station.CityId,
                CityName = station.City?.Name,
                CompanyId = station.CompanyId,
                CompanyName = station.Company?.Name
            };
        }

        public static IEnumerable<StationDto> ToDtoList(this IEnumerable<Station> stations)
        {
            return stations?.Select(s => s.ToDto()).ToList();
        }

        public static Station ToEntity(this StationDto stationDto)
        {
            if (stationDto == null)
                return null;

            return new Station
            {
                Id = stationDto.Id,
                Name = stationDto.Name,
                Latitude = stationDto.Latitude,
                Longitude = stationDto.Longitude,
                CityId = stationDto.CityId,
                CompanyId = stationDto.CompanyId,
                IsDeleted = false
            };
        }

        public static Station ToEntity(this StationAddUpdateDto stationDto)
        {
            if (stationDto == null)
                return null;

            return new Station
            {
                Name = stationDto.Name,
                Latitude = stationDto.Latitude,
                Longitude = stationDto.Longitude,
                CityId = stationDto.CityId,
                CompanyId = null, // System stations always have CompanyId = null
                IsDeleted = false
            };
        }

        public static Station ToEntity(this StationDto stationDto, Station existingStation)
        {
            if (stationDto == null || existingStation == null)
                return existingStation;

            existingStation.Name = stationDto.Name;
            existingStation.Latitude = stationDto.Latitude;
            existingStation.Longitude = stationDto.Longitude;
            existingStation.CityId = stationDto.CityId;
            existingStation.CompanyId = stationDto.CompanyId;

            return existingStation;
        }

        public static Station ToEntity(this StationAddUpdateDto stationDto, Station existingStation)
        {
            if (stationDto == null || existingStation == null)
                return existingStation;

            existingStation.Name = stationDto.Name;
            existingStation.Latitude = stationDto.Latitude;
            existingStation.Longitude = stationDto.Longitude;
            existingStation.CityId = stationDto.CityId;
            existingStation.CompanyId = null; // System stations always have CompanyId = null

            return existingStation;
        }

        // New method to convert a station to a DTO for display within cities
        public static StationInCityDto ToStationInCityDto(this Station station)
        {
            if (station == null)
                return null;

            return new StationInCityDto
            {
                Id = station.Id,
                Name = station.Name,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                CompanyName = station.Company?.Name
            };
        }

        // Method to convert a collection of stations to a list of DTOs for display within cities
        public static IEnumerable<StationInCityDto> ToStationInCityDtoList(this IEnumerable<Station> stations)
        {
            return stations?.Select(s => s.ToStationInCityDto()).ToList();
        }

        // Convert CompanyStationAddUpdateDto to Station entity
        public static Station ToEntity(this CompanyStationAddUpdateDto companyStationDto)
        {
            if (companyStationDto == null)
                return null;

            return new Station
            {
                Name = companyStationDto.Name,
                Latitude = companyStationDto.Latitude,
                Longitude = companyStationDto.Longitude,
                CityId = companyStationDto.CityId,
                CompanyId = companyStationDto.CompanyId, // CompanyId is required here
                IsDeleted = false
            };
        }

        // Update existing Station entity using CompanyStationAddUpdateDto
        public static Station ToEntity(this CompanyStationAddUpdateDto companyStationDto, Station existingStation)
        {
            if (companyStationDto == null || existingStation == null)
                return existingStation;

            existingStation.Name = companyStationDto.Name;
            existingStation.Latitude = companyStationDto.Latitude;
            existingStation.Longitude = companyStationDto.Longitude;
            existingStation.CityId = companyStationDto.CityId;
            existingStation.CompanyId = companyStationDto.CompanyId; // CompanyId is required here

            return existingStation;
        }
    }
}