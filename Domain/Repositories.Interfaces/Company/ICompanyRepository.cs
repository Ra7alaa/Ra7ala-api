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

        // Retrieve a company by its ID
        Task<Company?> GetCompanyByIdAsync(int id);

        // Check if a user has access to a specific company based on their ID and the company's ID
        Task<bool> UserHasAccessToCompany(string userId, int companyId);

        // Retrieve a list of companies that are pending approval
        Task<List<Company>> GetPendingCompaniesAsync();

        // Update a company's status (approved or rejected) with an optional rejection reason
        Task<bool> UpdateCompanyStatusAsync(int id, bool isApproved, string? rejectionReason = null);

        // Get paginated list of companies with optional filter
        Task<(IEnumerable<Company> Companies, int TotalCount)> GetPagedCompaniesAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Company, bool>>? filter = null);

        // Retrieves a company by its ID, with its feedbacks and the passengers who submitted them
        Task<Company?> GetCompanyWithRatingsAsync(int id);

        // Update the company's rating based on the company ID and the new rating
        Task<bool> UpdateCompanyRatingAsync(int companyId);

        // Get the average rating of a company
        Task<int> GetAverageRatingAsync(int companyId);

        // Check if a company exists that matches the given condition
        Task<bool> ExistsAsync(Expression<Func<Company, bool>> predicate);
        Task<List<Domain.Entities.Company>> GetAllCompaniesAsync();
        
    }
}