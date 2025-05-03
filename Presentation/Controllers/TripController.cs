using Application.DTOs.Trip;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        /// <summary>
        /// Creates a new trip based on a route.
        /// </summary>
        /// <param name="createDto">The trip creation data including route, driver, and station times.</param>
        /// <returns>The created trip details.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ApiValidationErrorResponse { Errors = validationErrors });
            }

            var result = await _tripService.CreateTripAsync(createDto);
            if (result.IsSuccess && result.Data != null)
            {
                var response = new ApiResponse(StatusCodes.Status201Created, "Trip created successfully")
                {
                    Data = result.Data
                };
                return CreatedAtAction(nameof(GetTrip), new { tripId = result.Data.Id }, response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Gets all trips for a specific company.
        /// </summary>
        /// <param name="companyId">The company ID.</param>
        /// <returns>A list of trips.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetTrips([FromQuery] int companyId)
        {
            if (companyId <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Company ID must be greater than 0" }
                });
            }

            var result = await _tripService.GetAllTripsByCompanyAsync(companyId);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Trips retrieved successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Gets paginated trips for a specific company.
        /// </summary>
        /// <param name="companyId">The company ID.</param>
        /// <param name="pageNumber">The page number (default is 1).</param>
        /// <param name="pageSize">The page size (default is 10).</param>
        /// <returns>A paginated list of trips.</returns>
        [HttpGet("paginated")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetPaginatedTrips(
            [FromQuery] int companyId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (companyId <= 0 || pageNumber < 1 || pageSize < 1)
            {
                var errors = new List<string>();
                if (companyId <= 0) errors.Add("Company ID must be greater than 0");
                if (pageNumber < 1) errors.Add("Page number must be greater than 0");
                if (pageSize < 1) errors.Add("Page size must be greater than 0");
                return BadRequest(new ApiValidationErrorResponse { Errors = errors });
            }

            var result = await _tripService.GetPaginatedTripsAsync(companyId, pageNumber, pageSize);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Trips retrieved successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Gets a specific trip by ID.
        /// </summary>
        /// <param name="tripId">The trip ID.</param>
        /// <returns>The trip details.</returns>
        [HttpGet("{tripId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetTrip(int tripId)
        {
            if (tripId <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Trip ID must be greater than 0" }
                });
            }

            var result = await _tripService.GetTripByIdAsync(tripId);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Trip retrieved successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound, result.Errors.First()));
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Gets all trips for a specific route.
        /// </summary>
        /// <param name="routeId">The route ID.</param>
        /// <returns>A list of trips for the specified route.</returns>
        [HttpGet("route/{routeId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        public async Task<IActionResult> GetTripsByRoute(int routeId)
        {
            if (routeId <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Route ID must be greater than 0" }
                });
            }

            var result = await _tripService.GetTripsByRouteAsync(routeId);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Trips retrieved successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Gets all trips assigned to a specific driver.
        /// </summary>
        /// <param name="driverId">The driver ID.</param>
        /// <returns>A list of trips assigned to the driver.</returns>
        [HttpGet("driver/{driverId}")]
        [Authorize(Roles = "Admin,SuperAdmin,Driver")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetTripsByDriver(string driverId)
        {
            if (string.IsNullOrEmpty(driverId))
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Driver ID cannot be empty" }
                });
            }

            var result = await _tripService.GetTripsByDriverAsync(driverId);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Trips retrieved successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Updates a trip's status (completed or not).
        /// </summary>
        /// <param name="updateDto">The trip status update data.</param>
        /// <returns>Success message on successful update.</returns>
        [HttpPatch("status")]
        [Authorize(Roles = "Admin,SuperAdmin,Driver")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateTripStatus([FromBody] UpdateTripStatusDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ApiValidationErrorResponse { Errors = validationErrors });
            }

            var result = await _tripService.UpdateTripStatusAsync(updateDto);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Trip status updated successfully");
                return Ok(response);
            }

            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound, result.Errors.First()));
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Deletes a specific trip.
        /// </summary>
        /// <param name="tripId">The trip ID.</param>
        /// <returns>Success message on successful deletion.</returns>
        [HttpDelete("{tripId}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DeleteTrip(int tripId)
        {
            if (tripId <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Trip ID must be greater than 0" }
                });
            }

            var result = await _tripService.DeleteTripAsync(tripId);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status204NoContent, "Trip deleted successfully");
                return StatusCode(StatusCodes.Status204NoContent, response);
            }

            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound, result.Errors.First()));
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Gets all future trips across all companies.
        /// </summary>
        /// <returns>A list of all upcoming trips with their details.</returns>
        [HttpGet("all")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        public async Task<IActionResult> GetAllFutureTrips()
        {
            var result = await _tripService.GetAllFutureTripsAsync();
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Future trips retrieved successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Searches for trips based on start city, end city, departure date, and required seats.
        /// </summary>
        /// <param name="searchDto">The search criteria.</param>
        /// <returns>A list of trips matching the search criteria.</returns>
        [HttpPost("search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        public async Task<IActionResult> SearchTrips([FromBody] SearchTripDto searchDto)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ApiValidationErrorResponse { Errors = validationErrors });
            }

            var result = await _tripService.SearchTripsAsync(
                searchDto.StartCityId,
                searchDto.EndCityId,
                searchDto.DepartureDate,
                searchDto.RequiredSeats);

            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Trips search completed successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }
    }
}