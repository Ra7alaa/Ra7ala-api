using Application.DTOs;
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
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        /// <summary>
        /// Creates a new route.
        /// </summary>
        /// <param name="createDto">The route creation data.</param>
        /// <returns>The created route details.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new ApiValidationErrorResponse { Errors = validationErrors });
            }

            var result = await _routeService.CreateRouteAsync(createDto);
            if (result.IsSuccess && result.Data != null)
            {
                var response = new ApiResponse(StatusCodes.Status201Created, "Route created successfully")
                {
                    Data = result.Data
                };
                return CreatedAtAction(nameof(GetRoute), new { routeId = result.Data.Id }, response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Gets all routes for a specific company.
        /// </summary>
        /// <param name="companyId">The company ID.</param>
        /// <returns>A list of routes.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetRoutes([FromQuery] int companyId)
        {
            if (companyId <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Company ID must be greater than 0" }
                });
            }

            var result = await _routeService.GetAllRoutesAsync(companyId);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Routes retrieved successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Gets paginated routes for a specific company.
        /// </summary>
        /// <param name="companyId">The company ID.</param>
        /// <param name="pageNumber">The page number (default is 1).</param>
        /// <param name="pageSize">The page size (default is 10).</param>
        /// <returns>A paginated list of routes.</returns>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetPaginatedRoutes(
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

            var result = await _routeService.GetPaginatedRoutesAsync(companyId, pageNumber, pageSize);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Routes retrieved successfully")
                {
                    Data = result.Data
                };
                return Ok(response);
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Gets a specific route by ID.
        /// </summary>
        /// <param name="routeId">The route ID.</param>
        /// <returns>The route details.</returns>
        [HttpGet("{routeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetRoute(int routeId)
        {
            if (routeId <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Route ID must be greater than 0" }
                });
            }

            var result = await _routeService.GetRouteByIdAsync(routeId);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status200OK, "Route retrieved successfully")
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
        /// Updates an existing route.
        /// </summary>
        /// <param name="routeId">The route ID.</param>
        /// <param name="updateDto">The route update data.</param>
        /// <returns>The updated route details.</returns>
        // [HttpPut("{id}")]
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        // [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        // [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        // public async Task<IActionResult> UpdateRoute(int id, [FromBody] UpdateRouteDto updateDto)
        // {
        //     if (id != updateDto.Id)
        //         return BadRequest(new
        //         {
        //             StatusCode = 400,
        //             Message = "Bad Request, you have made",
        //             Errors = new[] { "Route ID in URL does not match ID in body" }
        //         });

        //     var result = await _routeService.UpdateRouteAsync(updateDto);
        //     if (!result.IsSuccess)
        //         return BadRequest(new
        //         {
        //             StatusCode = 400,
        //             Message = "Bad Request, you have made",
        //             Errors = result.Errors
        //         });

        //     return Ok(new
        //     {
        //         StatusCode = 200,
        //         Message = "Route updated successfully",
        //         Data = result.Data
        //     });
        // }

        /// <summary>
        /// Deletes a specific route.
        /// </summary>
        /// <param name="routeId">The route ID.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{routeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiValidationErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DeleteRoute(int routeId)
        {
            if (routeId <= 0)
            {
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Route ID must be greater than 0" }
                });
            }

            var result = await _routeService.DeleteRouteAsync(routeId);
            if (result.IsSuccess)
            {
                var response = new ApiResponse(StatusCodes.Status204NoContent, "Route deleted successfully");
                return StatusCode(StatusCodes.Status204NoContent, response);
            }

            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(new ApiResponse(StatusCodes.Status404NotFound, result.Errors.First()));
            }

            return BadRequest(new ApiValidationErrorResponse { Errors = result.Errors });
        }
    }
}