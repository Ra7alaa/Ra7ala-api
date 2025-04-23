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

        [HttpPost]
        public async Task<ActionResult<StationDto>> CreateStation(StationDto stationDto)
        {
            await _stationService.AddStation(stationDto);
            return CreatedAtAction(nameof(GetStation), new { id = stationDto.Id }, stationDto);
        }

        [HttpPost("AddStations")]
        public async Task<ActionResult<IEnumerable<StationDto>>> AddStations([FromForm] List<StationDto> stationDtos)

        {
            await _stationService.AddStationsAsync(stationDtos);
            return Ok(stationDtos);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStation(int id, StationDto stationDto)
        {
            var result = await _stationService.UpdateStation(id, stationDto);
            if (!result)
            {
                return NotFound();
            }
            return Ok(stationDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var result = await _stationService.DeleteStation(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }


    }
}
