using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Company;

namespace Application.Services.Interfaces
{
   public interface ICompanyService
    {
        // Retrieves a paginated list of companies.
        Task<CompanyListResponseDto> GetCompaniesAsync(int pageNumber , int pageSize );

        // Retrieves a specific company by its ID.
        Task<CompanyDto> GetCompanyByIdAsync(int id);

        // Creates a new company using the provided data.
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto);

        // Updates an existing company with the provided data.
        Task<CompanyDto> UpdateCompanyAsync(int id, UpdateCompanyDto updateCompanyDto);

        // Deletes a company by its ID.
        Task<bool> DeleteCompanyAsync(int id);

        // Reviews a company's registration based on the provided details.
        Task<CompanyDto> ReviewCompanyRegistrationAsync(ReviewCompanyDto reviewDto);

        // Allows users to rate a company and add a comment.
        Task<CompanyDto> RateCompanyAsync(int companyId, int rating, string? comment, string userId);

        // Retrieves a list of companies that are pending approval or registration.
        Task<List<CompanyDto>> GetPendingCompaniesAsync();

        // Calculates the average rating for a given company.
        Task<double> GetCompanyAverageRatingAsync(int companyId);
    }
}