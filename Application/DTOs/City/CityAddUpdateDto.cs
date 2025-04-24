using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Station;

namespace Application.DTOs.City
{
    public class CityAddUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Governorate { get; set; } = string.Empty;
        
        // Propiedad opcional para agregar estaciones al crear/actualizar una ciudad
        public List<StationAddUpdateDto>? Stations { get; set; }
    }
}