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

        [HttpPost]
        public async Task<IActionResult> AddCity([FromBody] CityDto cityDto)
        {
            await _cityService.AddCityAsync(cityDto);
            return Ok(cityDto);
        }

        [HttpPost("AddCities")]
        public async Task<IActionResult> AddCities([FromBody] List<CityDto> cities)
        {
            await _cityService.AddCitiesAsync(cities);
            return Ok(cities);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityDto cityDto)
        {
          var result =  await _cityService.UpdateCityAsync(id, cityDto);

             if (!result)
            {
                return NotFound();
            }
            
            return Ok(cityDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var result = await _cityService.DeleteCityAsync(id);

            if (!result)
            {
                return NotFound();
            }
            
            return Ok();
        }

        // Add other actions as needed

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
    }
}
