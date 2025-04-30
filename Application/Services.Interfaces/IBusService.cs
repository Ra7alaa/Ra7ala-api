using Application.DTOs.Bus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IBusService
    {
        Task<IEnumerable<BusDto>> GetAllBusesAsync();
        Task<BusDto?> GetBusByIdAsync(int id);
        Task<BusDto> CreateBusAsync(CreateBusDto createBusDto);
        Task<BusDto?> UpdateBusAsync(int id, UpdateBusDto updateBusDto);
        Task<bool> DeleteBusAsync(int id);
         

   
        Task<IEnumerable<BusDto>> GetBusesByCompanyAsync(int companyId);
        Task<IEnumerable<BusDto>> GetActiveBusesAsync();
        Task<bool> SoftDeleteBusAsync(int id);
        Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, int? excludeBusId = null);
    }
    }

