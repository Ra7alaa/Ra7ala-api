using Application.DTOs.Station;
using Application.Services.Interfaces;
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

        // [HttpGet("GetNearbyStations/{latitude}/{longitude}/{radiusInKm}")]
        // public async Task<IEnumerable<StationDto>> GetNearbyStations(double latitude, double longitude, double radiusInKm) => await _stationService.GetNearbyStations(latitude, longitude, radiusInKm);
        
        [HttpPost]
        public async Task<ActionResult<StationDto>> AddStation(StationAddUpdateDto stationDto)
        {
            var result = await _stationService.AddStation(stationDto);
            return CreatedAtAction(nameof(GetStation), new { id = result.Id }, result);
        }

        [HttpPost("AddStations")]
        public async Task<ActionResult<IEnumerable<StationDto>>> AddStations([FromBody] List<StationAddUpdateDto> stationDtos)
        {
            var result = await _stationService.AddStationsAsync(stationDtos);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStation(int id, StationAddUpdateDto stationDto)
        {
            var result = await _stationService.UpdateStation(id, stationDto);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var result = await _stationService.DeleteStation(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
