using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Station;

namespace Application.DTOs.City
{
    public class CityDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Governorate { get; set; } = string.Empty;
        
        // استخدام DTO المخصص للمحطات ضمن المدن
        public List<StationInCityDto>? Stations { get; set; }
    }
}
