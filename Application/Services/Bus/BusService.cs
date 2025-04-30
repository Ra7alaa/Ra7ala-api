using Application.DTOs.Bus;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bus = Domain.Entities.Bus;  // Add this at the top with other using statements
namespace Application.Services.Bus

{
    public class BusService : IBusService
    {

        private readonly IUnitOfWork _unitOfWork;

        public BusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BusDto>> GetAllBusesAsync()
        {
            var buses = await _unitOfWork.Buses.GetAllAsync();
            return buses.Select(bus => new BusDto
            {
                Id = bus.Id,
                RegistrationNumber = bus.RegistrationNumber,
                Model = bus.Model,
                Capacity = bus.Capacity,
                AmenityDescription = bus.AmenityDescription,
                IsActive = bus.IsActive,
                CompanyId = bus.CompanyId
            });
        }

        public async Task<BusDto?> GetBusByIdAsync(int id)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);
            if (bus == null) return null;

            return new BusDto
            {
                Id = bus.Id,
                RegistrationNumber = bus.RegistrationNumber,
                Model = bus.Model,
                Capacity = bus.Capacity,
                AmenityDescription = bus.AmenityDescription,
                IsActive = bus.IsActive,
                CompanyId = bus.CompanyId
            };
        }

        public async Task<BusDto> CreateBusAsync(CreateBusDto createBusDto)
        {
            var bus = new Domain.Entities.Bus
            {
                RegistrationNumber = createBusDto.RegistrationNumber,
                Model = createBusDto.Model,
                Capacity = createBusDto.Capacity,
                AmenityDescription = createBusDto.AmenityDescription,
                CompanyId = createBusDto.CompanyId,
                IsActive = true
            };

            await _unitOfWork.Buses.AddAsync(bus);
            await _unitOfWork.SaveChangesAsync();

            return new BusDto
            {
                Id = bus.Id,
                RegistrationNumber = bus.RegistrationNumber,
                Model = bus.Model,
                Capacity = bus.Capacity,
                AmenityDescription = bus.AmenityDescription,
                IsActive = bus.IsActive,
                CompanyId = bus.CompanyId
            };
        }

        public async Task<BusDto?> UpdateBusAsync(int id, UpdateBusDto updateBusDto)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);
            if (bus == null) return null;

            bus.RegistrationNumber = updateBusDto.RegistrationNumber;
            bus.Model = updateBusDto.Model;
            bus.Capacity = updateBusDto.Capacity;
            bus.AmenityDescription = updateBusDto.AmenityDescription;
            bus.IsActive = updateBusDto.IsActive;

            _unitOfWork.Buses.Update(bus);
            await _unitOfWork.SaveChangesAsync();

            return new BusDto
            {
                Id = bus.Id,
                RegistrationNumber = bus.RegistrationNumber,
                Model = bus.Model,
                Capacity = bus.Capacity,
                AmenityDescription = bus.AmenityDescription,
                IsActive = bus.IsActive,
                CompanyId = bus.CompanyId
            };
        }

        public async Task<bool> DeleteBusAsync(int id)
        {
            var bus = await _unitOfWork.Buses.GetByIdAsync(id);
            if (bus == null) return false;

            _unitOfWork.Buses.Remove(bus);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
  
  
  public async Task<IEnumerable<BusDto>> GetBusesByCompanyAsync(int companyId)
{
    var buses = await _unitOfWork.Buses.GetBusesByCompanyIdAsync(companyId);
    return buses.Select(bus => new BusDto
    {
        Id = bus.Id,
        RegistrationNumber = bus.RegistrationNumber,
        Model = bus.Model,
        Capacity = bus.Capacity,
        AmenityDescription = bus.AmenityDescription,
        IsActive = bus.IsActive,
        CompanyId = bus.CompanyId
    });
}
  
  
 
  
public async Task<IEnumerable<BusDto>> GetActiveBusesAsync()
{
    var buses = await _unitOfWork.Buses.GetAllAsync();
    return buses.Where(b => b.IsActive).Select(bus => new BusDto
    {
        Id = bus.Id,
        RegistrationNumber = bus.RegistrationNumber,
        Model = bus.Model,
        Capacity = bus.Capacity,
        AmenityDescription = bus.AmenityDescription,
        IsActive = bus.IsActive,
        CompanyId = bus.CompanyId
    });
}

public async Task<bool> SoftDeleteBusAsync(int id)
{
    var bus = await _unitOfWork.Buses.GetByIdAsync(id);
    if (bus == null) return false;

    _unitOfWork.Buses.SoftDelete(bus);
    await _unitOfWork.SaveChangesAsync();
    return true;
}


public async Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, int? excludeBusId = null)
{
    var buses = await _unitOfWork.Buses.GetAllAsync();
    return !buses.Any(b => 
        b.RegistrationNumber == registrationNumber && 
        (!excludeBusId.HasValue || b.Id != excludeBusId.Value));
}






    }
}
