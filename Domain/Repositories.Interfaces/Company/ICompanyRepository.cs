using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Entities;
using Company = Domain.Entities.Company; // Alias to avoid confusion with other Company classes 

namespace Domain.Repositories.Interfaces
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        // Retrieve a company with additional details by its ID
        Task<Company> GetCompanyWithDetailsAsync(int id);
        
        // Retrieve a list of companies that are pending approval
        Task<List<Company>> GetPendingCompaniesAsync();
        
        // Update a company's status (approved or rejected) with an optional rejection reason
        Task<bool> UpdateCompanyStatusAsync(int id, bool isApproved, string? rejectionReason = null);
        
        // Update the company's rating based on the company ID and the new rating
        Task<bool> UpdateCompanyRatingAsync(int companyId, double newRating);
        
        // Get paginated list of companies with optional filter
        Task<(IEnumerable<Company> Companies, int TotalCount)> GetPagedCompaniesAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Company, bool>>? filter = null);


        // Get a company by ID including its ratings
        Task<Company?> GetCompanyWithRatingsAsync(int id);

        // Get the average rating of a company
        Task<double> GetAverageRatingAsync(int companyId);

        // Check if a company exists by name and email
        Task<bool> ExistsAsync(string name, string email);
    }
}