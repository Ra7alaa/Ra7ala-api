using Application.DTOs.Station;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationController(IStationService stationService)
        {
            this._stationService = stationService;
        }

        [HttpGet]
        public async Task<IEnumerable<StationDto>> GetStations() => await _stationService.GetStations();

        [HttpGet("{id}")]
        public async Task<ActionResult<StationDto>> GetStation(int id)
        {
            var station = await _stationService.GetStationById(id);
            if (station == null)
            {
                return NotFound();
            }
            return station;
        }

        [HttpGet("GetStationsByCity/{cityName}")]
        public async Task<IEnumerable<StationDto>> GetStationsByCityName(string cityName) => await _stationService.GetStationsByCityName(cityName);

        [HttpGet("GetStationsByCompanyId/{companyId}")]
        public async Task<IEnumerable<StationDto>> GetStationsByCompanyId(int companyId) => await _stationService.GetStationsByCompanyId(companyId);

        // Add new functions to distinguish between system stations and company stations
        [HttpGet("system")]
        public async Task<IEnumerable<StationDto>> GetSystemStations() => await _stationService.GetSystemStations();
        
        [HttpGet("company/{companyId}")]
        public async Task<IEnumerable<StationDto>> GetCompanyStations(int companyId) => await _stationService.GetCompanyStations(companyId);

        // New endpoint to get system stations and specified company stations
        [HttpGet("system-and-company/{companyId}")]
        public async Task<IEnumerable<StationDto>> GetSystemAndCompanyStations(int companyId) => await _stationService.GetSystemAndCompanyStations(companyId);

        // [HttpGet("GetNearbyStations/{latitude}/{longitude}/{radiusInKm}")]
        // public async Task<IEnumerable<StationDto>> GetNearbyStations(double latitude, double longitude, double radiusInKm) => await _stationService.GetNearbyStations(latitude, longitude, radiusInKm);

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<StationDto>> AddStation(StationAddUpdateDto stationDto)
        {
            try
            {
                var result = await _stationService.AddStation(stationDto);
                return CreatedAtAction(nameof(GetStation), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // New endpoint to add a company station
        [HttpPost("company")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<StationDto>> AddCompanyStation(CompanyStationAddUpdateDto companyStationDto)
        {
            try
            {
                var result = await _stationService.AddCompanyStation(companyStationDto);
                return CreatedAtAction(nameof(GetStation), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddStations")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<IEnumerable<StationDto>>> AddStations([FromBody] List<StationAddUpdateDto> stationDtos)
        {
            try
            {
                var result = await _stationService.AddStationsAsync(stationDtos);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // New endpoint to add a batch of company stations
        [HttpPost("company/batch")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<IEnumerable<StationDto>>> AddCompanyStations([FromBody] List<CompanyStationAddUpdateDto> companyStationDtos)
        {
            try
            {
                var result = await _stationService.AddCompanyStationsAsync(companyStationDtos);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateStation(int id, StationAddUpdateDto stationDto)
        {
            try
            {
                var result = await _stationService.UpdateStation(id, stationDto);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // New endpoint to update a company station
        [HttpPut("company/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateCompanyStation(int id, CompanyStationAddUpdateDto companyStationDto)
        {
            try
            {
                var result = await _stationService.UpdateCompanyStation(id, companyStationDto);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner,Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var result = await _stationService.DeleteStation(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        
        // Endpoint for deleting a system station (Owner only)
        [HttpDelete("system/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteSystemStation(int id)
        {
            try
            {
                var result = await _stationService.DeleteSystemStation(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // Endpoint for deleting a company station (Admin and SuperAdmin)
        [HttpDelete("company/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteCompanyStation(int id)
        {
            try
            {
                // Extract company ID from user claims
                var companyIdClaim = User.FindFirst("CompanyId")?.Value;
                if (string.IsNullOrEmpty(companyIdClaim))
                {
                    return BadRequest("Company ID is not available");
                }
                
                if (!int.TryParse(companyIdClaim, out int companyId))
                {
                    return BadRequest("Invalid company ID format");
                }
                
                var result = await _stationService.DeleteCompanyStation(id, companyId);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
