using Application.DTOs.Company;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations; 

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(
            ICompanyService companyService,
            ILogger<CompanyController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        //Create a new company
        // [SwaggerOperation(
        // Summary = "Creates a new company",
        // Description = "Allows any user to submit a new company request. Owner reviews later.",
        // OperationId = "CreateCompany",
        // Tags = new[] { "Company Management" }
        // )]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompanyDto>> CreateCompany([FromForm] CreateCompanyDto createDto)
        {
            try
            {
                var company = await _companyService.CreateCompanyAsync(createDto);
                return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                //   if (_environment.IsDevelopment())
                //     {
                //         return StatusCode(500, $"Error details: {ex.Message}, Stack: {ex.StackTrace}");
                //     }
                //     return StatusCode(500, "An error occurred while creating the company");
                return StatusCode(500, "An error occurred while creating the company");
            }
        }

        // Get all companies with pagination   
        // [SwaggerOperation(
        //     Summary = "Get all companies",
        //     Description = "Returns a paginated list of all companies.",
        //     OperationId = "GetCompanies",
        //     Tags = new[] { "Company Management" }
        // )]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<CompanyListResponseDto>> GetCompanies(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _companyService.GetAllCompaniesAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies");
                return StatusCode(500, "An error occurred while retrieving companies");
            }
        }


        // Get pending companies awaiting approval
        // [SwaggerOperation(
        //     Summary = "Get pending companies",
        //     Description = "Returns all companies that are waiting for approval.",
        //     OperationId = "GetPendingCompanies",
        //     Tags = new[] { "Company Approval" }
        // )]
        [HttpGet("pending")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CompanyListResponseDto>> GetPendingCompanies(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var companies = await _companyService.GetPendingCompaniesAsync(pageNumber, pageSize);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending companies");
                return StatusCode(500, "An error occurred while retrieving pending companies");
            }
        }

        // Get approved companies
        // [SwaggerOperation(
        //     Summary = "Get approved companies",
        //     Description = "Retrieves all companies that have been approved.",
        //     OperationId = "GetApprovedCompanies",
        //     Tags = new[] { "Company Approval" }
        // )]
        [HttpGet("approved")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CompanyListResponseDto>> GetApprovedCompanies(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var companies = await _companyService.GetApprovedCompaniesAsync(pageNumber, pageSize);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approved companies");
                return StatusCode(500, "An error occurred while retrieving approved companies");
            }
        }

        // Get rejected companies
        // [SwaggerOperation(
        //     Summary = "Get rejected companies",
        //     Description = "Retrieves companies that were rejected in the approval process.",
        //     OperationId = "GetRejectedCompanies",
        //     Tags = new[] { "Company Approval" }
        // )]
        [HttpGet("rejected")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CompanyListResponseDto>> GetRejectedCompanies(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var companies = await _companyService.GetRejectedCompaniesAsync(pageNumber, pageSize);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rejected companies");
                return StatusCode(500, "An error occurred while retrieving rejected companies");
            }
        }
        // Get all companies with filtering and pagination
        // [SwaggerOperation(
        //     Summary = "Get filtered companies",
        //     Description = "Filters companies by approval status and other criteria with pagination support.",
        //     OperationId = "GetFilteredCompanies",
        //     Tags = new[] { "Company Management" }
        // )]
        [HttpGet("filter")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CompanyListResponseDto>> GetFilteredCompanies(
            [FromQuery] CompanyFilterDto filter,
            [FromQuery] bool? isPending = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (isPending.HasValue && isPending.Value)
                {
                    // Override filter settings for pending companies
                    filter ??= new CompanyFilterDto();
                    filter.IsApproved = false;
                    filter.IsRejected = false;
                    filter.IsDeleted = false;
                }

                var result = await _companyService.GetCompaniesAsync(pageNumber, pageSize, filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered companies");
                return StatusCode(500, "An error occurred while retrieving companies");
            }
        }

        // Get company by ID
        // [SwaggerOperation(
        //     Summary = "Get a company by ID",
        //     Description = "Retrieves details of a specific company using its ID",
        //     OperationId = "GetCompanyById",
        //     Tags = new[] { "Company Management" }
        // )]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
    
        public async Task<ActionResult<CompanyDto>> GetCompany(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                return Ok(company);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company {Id}", id);
                return StatusCode(500,  ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        // Get company user profile
        // [SwaggerOperation(
        //     Summary = "Get company user profile",
        //     Description = "Retrieves the user profile of a specific company.",
        //     OperationId = "GetCompanyUserProfile",
        //     Tags = new[] { "Company Management" }
        // )]
        [HttpGet("{id}/Company-User-profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompanyUserProfileDto>> GetCompanyUserProfile(int id)
        {
            try
            {
                var profile = await _companyService.GetCompanyUserProfileAsync(id);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company user profile for company {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the company profile");
            }
        }

        // Get company admin profile by ID
        // [SwaggerOperation(
        //     Summary = "Get company admin profile",
        //     Description = "Retrieves the admin profile of a specific company.",
        //     OperationId = "GetCompanyAdminProfile",
        //     Tags = new[] { "Company Management" }
        // )]
        [HttpGet("{id}/SuperAdmin-profile")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CompanyAdminProfileDto>> GetCompanyAdminProfile(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var profile = await _companyService.GetCompanyAdminProfileAsync(id, userId);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company admin profile for company {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the company profile");
            }
        }

        // Update company details
        // [SwaggerOperation(
        //     Summary = "Update company information",
        //     Description = "Updates the details of an existing company by its ID.",
        //     OperationId = "UpdateCompany",
        //     Tags = new[] { "Company Management" }
        // )]
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompanyDto>> UpdateCompany(int id, [FromForm] UpdateCompanyDto updateDto)
        {
            try
            {
                var company = await _companyService.UpdateCompanyAsync(id, updateDto);
                return Ok(company);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company {Id}", id);
                return StatusCode(500, "An error occurred while updating the company");
            }
        }

        // Review a company registration (approve or reject)
        // [SwaggerOperation(
        //     Summary = "Review company registration",
        //     Description = "Allows admin/owner to approve or reject company registration.",
        //     OperationId = "ReviewCompany",
        //     Tags = new[] { "Company Approval" }
        // )]
        [HttpPost("review")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompanyDto>> ReviewCompany([FromBody] ReviewCompanyDto reviewDto)
        {
            try
            {
                var company = await _companyService.ReviewCompanyRegistrationAsync(reviewDto);
                return Ok(company);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reviewing company {Id}", reviewDto.CompanyId);
                return StatusCode(500, "An error occurred while reviewing the company registration");
            }
        }

        // Delete a company (soft delete)
        //     [SwaggerOperation(
        //        Summary = "Delete a company",
        //        Description = "Soft deletes a company by setting deletion flags. Only accessible by authorized roles.",
        //        OperationId = "DeleteCompany",
        //        Tags = new[] { "Company Management" }
        //    )]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCompany(int id)
        {
            try
            {
                var result = await _companyService.DeleteCompanyAsync(id);
                if (!result)
                    return NotFound($"Company with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company {Id}", id);
                return StatusCode(500,"An error occurred while deleting the company");
            }
        }


        // Add Feedback
        // [SwaggerOperation(
        //     Summary = "Add Feedback",
        //     Description = "Allows a passenger to rate and comment on a company.",
        //     OperationId = "AddFeedback",
        //     Tags = new[] { "Company Rating" }
        // )]
        [HttpPost("{companyId}/Feedback")]
        [Authorize(Roles = "Passenger")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddFeedback(
            [FromRoute] int companyId,
            [FromBody] RateCompanyDto rateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not authenticated");

                await _companyService.RateCompanyAsync(companyId, rateDto.Rating, rateDto.Comment, userId);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rating company {CompanyId}", companyId);
                return StatusCode(500, "An error occurred while rating the company");
            }
        }


        // Get average rating for a company
        // [SwaggerOperation(
        //     Summary = "Get average company rating",
        //     Description = "Retrieves the average rating for a company by ID.",
        //     OperationId = "GetCompanyRating",
        //     Tags = new[] { "Company Rating" }
        // )]

        [HttpGet("{id}/average-rating ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<double>> GetCompanyRating(int id)
        {
            try
            {
                var rating = await _companyService.GetCompanyAverageRatingAsync(id);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rating for company {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the company rating");
            }
        }

        // Get detailed ratings for a company
        // [SwaggerOperation(
        //     Summary = "Get company ratings details",
        //     Description = "Retrieves detailed ratings and feedback for a specific company.",
        //     OperationId = "GetCompanyRatingsDetails",
        //     Tags = new[] { "Company Rating" }
        // )]
        [HttpGet("{id}/ratings-details")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CompanyRatingsDto>> GetCompanyRatingsDetails(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var ratings = await _companyService.GetCompanyRatingsDetailsAsync(id);
                return Ok(ratings);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings details for company {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the company ratings");
            }
        }

    }

    }