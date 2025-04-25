using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Company;

namespace Application.Services.Interfaces
{
   public interface ICompanyService
    {
        // Creates a new company using the provided data.
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto);

        // Retrieves a paginated list of companies.
        Task<CompanyListResponseDto> GetAllCompaniesAsync(int pageNumber, int pageSize);
        
        // Retrieves a paginated list of companies that are pending.
        Task<CompanyListResponseDto> GetPendingCompaniesAsync(int pageNumber = 1, int pageSize = 10);
        
        // Retrieves a paginated list of companies that have been approved.
        Task<CompanyListResponseDto> GetApprovedCompaniesAsync(int pageNumber = 1, int pageSize = 10);
        
        // Retrieves a paginated list of companies that have been rejected.
        Task<CompanyListResponseDto> GetRejectedCompaniesAsync(int pageNumber = 1, int pageSize = 10);
        
        // Retrieves a paginated list of companies with Filter.
        Task<CompanyListResponseDto> GetCompaniesAsync(int pageNumber, int pageSize, CompanyFilterDto? filter = default);

        // Retrieves a specific company by its ID.
        Task<CompanyDto> GetCompanyByIdAsync(int id);

        // Updates an existing company with the provided data.
        Task<CompanyDto> UpdateCompanyAsync(int id, UpdateCompanyDto updateCompanyDto);

        // Deletes a company by its ID.
        Task<bool> DeleteCompanyAsync(int id);

        // Reviews a company's registration based on the provided details.
        Task<CompanyDto> ReviewCompanyRegistrationAsync(ReviewCompanyDto reviewDto);

        // Allows users to rate a company and add a comment.
        Task RateCompanyAsync(int companyId, int rating, string? comment, string userId);

        // Calculates the average rating for a given company.
        Task<int> GetCompanyAverageRatingAsync(int companyId);
    }
}