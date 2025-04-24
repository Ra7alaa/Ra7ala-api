using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.City;
using Application.Map;
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
            return cities.ToCityDtoList();
        }

        public async Task<CityDto?> GetCityByIdAsync(int id)
        {
            var city = await _unitOfWork.Cities.GetByIdAsync(id);
            return city?.ToCityDto();
        }

        public async Task<CityDto> AddCityAsync(CityAddUpdateDto cityDto)
        {
            var city = cityDto.ToCity();
            await _unitOfWork.Cities.AddAsync(city);
            await _unitOfWork.SaveChangesAsync();
            
            // ارجاع DTO كامل مع المعرف الجديد
            return city.ToCityDto();
        }

        public async Task<List<CityDto>> AddCitiesAsync(List<CityAddUpdateDto> citiesDto)
        {
            // استخدام LINQ للتحويل (من DTO إلى Entity)
            var cities = citiesDto.Select(dto => dto.ToCity()).ToList();

            await _unitOfWork.Cities.AddRangeAsync(cities);
            await _unitOfWork.SaveChangesAsync();
            
            // ارجاع قائمة DTOs كاملة مع المعرفات الجديدة
            return cities.Select(c => c.ToCityDto()).ToList();
        }

        public async Task<CityDto?> UpdateCityAsync(int id, CityAddUpdateDto cityDto)
        {
            var city = await _unitOfWork.Cities.GetByIdAsync(id);
            
            if (city == null)
                return null;

            // تحديث خصائص الكيان
            city.Name = cityDto.Name;
            city.Governorate = cityDto.Governorate;
            
            _unitOfWork.Cities.Update(city);
            await _unitOfWork.SaveChangesAsync();
            
            // ارجاع DTO كامل بعد التحديث
            return city.ToCityDto();
        }

        public async Task<bool> DeleteCityAsync(int id)
        {
            var city = await _unitOfWork.Cities.GetByIdAsync(id);
            
            if (city == null)
                return false;

            // Using soft delete instead of hard delete
            city.IsDeleted = true;
            _unitOfWork.Cities.Update(city);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CityDto>> GetCitiesByGovernorateAsync(string governorate)
        {
            var cities = await _unitOfWork.Cities.GetCitiesByGovernorateAsync(governorate);
            return cities.ToCityDtoList();
        }
    }
}
