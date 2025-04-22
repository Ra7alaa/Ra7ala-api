using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Company;

namespace Application.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyListResponseDto> GetCompaniesAsync(int pageNumber, int pageSize);
        Task<CompanyDto> GetCompanyByIdAsync(int id);
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto);
        Task<CompanyDto> UpdateCompanyAsync(int id, UpdateCompanyDto updateCompanyDto);
        Task<bool> DeleteCompanyAsync(int id);
        Task<CompanyDto> ReviewCompanyRegistrationAsync(ReviewCompanyDto reviewDto);
        Task<CompanyDto> RateCompanyAsync(int companyId, int rating, string? comment, string userId);
    }
}