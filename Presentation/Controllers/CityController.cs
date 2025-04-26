using Application.DTOs.City;
using Application.Services.City;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            var city = await _cityService.GetCityByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity([FromBody] CityAddUpdateDto cityDto)
        {
            var result = await _cityService.AddCityAsync(cityDto);
            return CreatedAtAction(nameof(GetCityById), new { id = result.Id }, result);
        }

        [HttpPost("AddCities")]
        public async Task<IActionResult> AddCities([FromBody] List<CityAddUpdateDto> cities)
        {
            var result = await _cityService.AddCitiesAsync(cities);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityAddUpdateDto cityDto)
        {
            var result = await _cityService.UpdateCityAsync(id, cityDto);

            if (result == null)
            {
                return NotFound();
            }
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var result = await _cityService.DeleteCityAsync(id);

            if (!result)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        // الحصول على جميع المدن مع محطاتها
        [HttpGet("with-stations")]
        public async Task<IActionResult> GetAllCitiesWithStations()
        {
            var cities = await _cityService.GetAllCitiesWithStationsAsync();
            return Ok(cities);
        }

        // الحصول على مدينة محددة مع محطاتها
        [HttpGet("{id}/with-stations")]
        public async Task<IActionResult> GetCityWithStationsById(int id)
        {
            var city = await _cityService.GetCityWithStationsByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city);
        }
    }
}
