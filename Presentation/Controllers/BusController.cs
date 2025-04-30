using Application.DTOs.Bus;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusController : ControllerBase
    {
        private readonly IBusService _busService;
        private readonly ILogger<BusController> _logger;

        public BusController(IBusService busService, ILogger<BusController> logger)
        {
            _busService = busService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<ActionResult<IEnumerable<BusDto>>> GetAllBuses()
        {
            try
            {
                var buses = await _busService.GetAllBusesAsync();
                return Ok(buses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all buses");
                return StatusCode(500, "An error occurred while retrieving buses");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<ActionResult<BusDto>> GetBusById(int id)
        {
            try
            {
                var bus = await _busService.GetBusByIdAsync(id);
                if (bus == null)
                    return NotFound($"Bus with ID {id} not found");

                return Ok(bus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bus {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the bus");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BusDto>> CreateBus( [FromForm]CreateBusDto createBusDto)
        {
            try
            {
                var isUnique = await _busService.IsRegistrationNumberUniqueAsync(createBusDto.RegistrationNumber);
                if (!isUnique)
                    return BadRequest("Registration number already exists");

                var bus = await _busService.CreateBusAsync(createBusDto);
                return CreatedAtAction(nameof(GetBusById), new { id = bus.Id }, bus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bus");
                return StatusCode(500, "An error occurred while creating the bus");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BusDto>> UpdateBus(int id, UpdateBusDto updateBusDto)
        {
            try
            {
                var isUnique = await _busService.IsRegistrationNumberUniqueAsync(updateBusDto.RegistrationNumber, id);
                if (!isUnique)
                    return BadRequest("Registration number already exists");

                var bus = await _busService.UpdateBusAsync(id, updateBusDto);
                if (bus == null)
                    return NotFound($"Bus with ID {id} not found");

                return Ok(bus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bus {Id}", id);
                return StatusCode(500, "An error occurred while updating the bus");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteBus(int id)
        {
            try
            {
                var result = await _busService.DeleteBusAsync(id);
                if (!result)
                    return NotFound($"Bus with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bus {Id}", id);
                return StatusCode(500, "An error occurred while deleting the bus");
            }
        }

        [HttpDelete("soft/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SoftDeleteBus(int id)
        {
            try
            {
                var result = await _busService.SoftDeleteBusAsync(id);
                if (!result)
                    return NotFound($"Bus with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting bus {Id}", id);
                return StatusCode(500, "An error occurred while soft deleting the bus");
            }
        }

        [HttpGet("company/{companyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<ActionResult<IEnumerable<BusDto>>> GetBusesByCompany(int companyId)
        {
            try
            {
                var buses = await _busService.GetBusesByCompanyAsync(companyId);
                return Ok(buses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting buses for company {CompanyId}", companyId);
                return StatusCode(500, "An error occurred while retrieving company buses");
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<ActionResult<IEnumerable<BusDto>>> GetActiveBuses()
        {
            try
            {
                var buses = await _busService.GetActiveBusesAsync();
                return Ok(buses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active buses");
                return StatusCode(500, "An error occurred while retrieving active buses");
            }
        }
    }
}