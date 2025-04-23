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
    }
}