using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Company = Domain.Entities.Company;

namespace Infrastructure.Repositories.Company
{
    public class CompanyRepository : GenericRepository<Domain.Entities.Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Retrieve a company with additional details by its ID
        // .Include(c => c.Trips)
        public async Task<Domain.Entities.Company> GetCompanyWithDetailsAsync(int id)
        {
            return await _context.Set<Domain.Entities.Company>()
                .Include(c => c.Admins)
                .Include(c => c.Drivers)
                .Include(c => c.Buses)
                .Include(c => c.Routes)
                .Include(c => c.Feedbacks)
                .Include(c => c.SuperAdmin)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

       

        // Get paginated list of companies with optional filter
        public async Task<(IEnumerable<Domain.Entities.Company> Companies, int TotalCount)> GetPagedCompaniesAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Domain.Entities.Company, bool>>? filter = null)
        {
            var query = _context.Set<Domain.Entities.Company>().AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            var companies = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (companies, totalCount);
        }

        public async Task<List<Domain.Entities.Company>> GetPendingCompaniesAsync()
        {
           return await _context.Set<Domain.Entities.Company>()
            .Where(c => !c.IsApproved && !c.IsRejected && !c.IsDeleted)
            .Include(c => c.SuperAdmin)
            .OrderByDescending(c => c.CreatedDate)
            .ToListAsync();
        }
       
        // Update a company's status (approved or rejected) with an optional rejection reason
        public async Task<bool> UpdateCompanyStatusAsync(int id, bool isApproved, string? rejectionReason = null)
        {
            var company = await _context.Set<Domain.Entities.Company>()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return false;

            company.IsApproved = isApproved;
            company.IsRejected = !isApproved;

            if (isApproved)
                company.ApprovedDate = DateTime.UtcNow;
            else if (rejectionReason != null)
                company.RejectionReason = rejectionReason;

            await _context.SaveChangesAsync();
            return true;
        }

        // Retrieves a company by its ID, with its feedbacks and the passengers who submitted them
        public async Task<Domain.Entities.Company?> GetCompanyWithRatingsAsync(int id)
        {
            return await _context.Set<Domain.Entities.Company>()
                .Include(c => c.Feedbacks)
                .ThenInclude(f => f.Passenger)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
       
        // Update the company's rating based on the company ID and the new rating
        public async Task<bool> UpdateCompanyRatingAsync(int companyId)
        {
            var company = await _context.Set<Domain.Entities.Company>()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                return false;

            var feedbacks = await _context.Set<CompanyFeedback>()
                .Where(f => f.CompanyId == companyId)
                .ToListAsync();

            if (!feedbacks.Any())
            {
                company.AverageRating = 0;
                company.TotalRatings = 0;
            }
            else
            {
                company.AverageRating = feedbacks.Average(f => f.Rating);
                company.TotalRatings = feedbacks.Count;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    
        // Get the average rating of a company
       public async Task<int> GetAverageRatingAsync(int companyId)
        {
            var feedbacks = await _context.Set<CompanyFeedback>()
                .Where(f => f.CompanyId == companyId)
                .ToListAsync();
            
            if (!feedbacks.Any())
                return 0; 
                
            double averageRating = feedbacks.Average(f => f.Rating);
            averageRating = Math.Max(0, averageRating);
            int starRating = (int)Math.Round(averageRating);
            starRating = Math.Min(5, starRating);
            
            return starRating;
        }

        // Check if a company exists that matches the given condition
       public async Task<bool> ExistsAsync(Expression<Func<Domain.Entities.Company, bool>> predicate)
        {
            return await _context.Set<Domain.Entities.Company>().AnyAsync(predicate);
        }
     }
}