using Application.DTOs.City;
using Application.DTOs.Station;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Map
{
    public static class CityMapper
    {
        public static CityDto ToCityDto(this City city)
        {
            return new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Governorate = city.Governorate,
                // استخدام ToStationInCityDtoList بدلاً من ToDto
                Stations = city.Stations?.Select(s => s.ToStationInCityDto()).ToList()
            };
        }

        public static City ToCity(this CityDto cityDto)
        {
            return new City
            {
                Id = cityDto.Id,
                Name = cityDto.Name,
                Governorate = cityDto.Governorate,
                IsDeleted = false
            };
        }

        public static City ToCity(this CityAddUpdateDto cityDto)
        {
            return new City
            {
                Name = cityDto.Name,
                Governorate = cityDto.Governorate,
                IsDeleted = false
                // No stations se crean aquí, ya que se gestionarán por separado
            };
        }

        public static IEnumerable<CityDto> ToCityDtoList(this IEnumerable<City> cities)
        {
            return cities.Select(c => c.ToCityDto());
        }
    }
}