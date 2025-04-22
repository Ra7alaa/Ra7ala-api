using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.City;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace Application.Services.City
{
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CityDto>> GetAllCitiesAsync()
        {
            var cities = await _unitOfWork.Cities.GetAllAsync();
            return cities
                .Where(c => !c.IsDeleted)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Governorate = c.Governorate
                });
        }

        public async Task<CityDto?> GetCityByIdAsync(int id)
        {
            var city = await _unitOfWork.Cities.GetByIdAsync(id);
            
            if (city == null || city.IsDeleted)
                return null; // Return null instead of throwing exception

            return new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Governorate = city.Governorate
            };
        }

        public async Task AddCityAsync(CityDto cityDto)
        {
            var city = new Domain.Entities.City
            {
                Name = cityDto.Name,
                Governorate = cityDto.Governorate,
                IsDeleted = false
            };

            await _unitOfWork.Cities.AddAsync(city);
            await _unitOfWork.SaveChangesAsync();
            
            // Update the DTO with the newly created ID
            cityDto.Id = city.Id;
        }

        public async Task AddCitiesAsync(List<CityDto> cities)
        {
            var citiesList = new List<Domain.Entities.City>();

            foreach (var cityDto in cities)
            {
                var city = new Domain.Entities.City
                {
                    Name = cityDto.Name,
                    Governorate = cityDto.Governorate,
                    IsDeleted = false
                };

                citiesList.Add(city);
            }

            await _unitOfWork.Cities.AddRangeAsync(citiesList);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateCityAsync(int id, CityDto cityDto)
        {
            // Use the id parameter instead of cityDto.Id
            var city = await _unitOfWork.Cities.GetByIdAsync(id);
            
            if (city == null || city.IsDeleted)
                return false; // Return false instead of throwing exception

            city.Name = cityDto.Name;
            city.Governorate = cityDto.Governorate;
            
            _unitOfWork.Cities.Update(city);
            await _unitOfWork.SaveChangesAsync();
            return true; // Return true to indicate success
        }

        public async Task<bool> DeleteCityAsync(int id)
        {
            var city = await _unitOfWork.Cities.GetByIdAsync(id);
            
            if (city == null || city.IsDeleted)
                return false; // Return false instead of throwing exception

            // Using soft delete instead of hard delete
            city.IsDeleted = true;
            _unitOfWork.Cities.Update(city);
            await _unitOfWork.SaveChangesAsync();
            return true; // Return true to indicate success
        }
    }
}
