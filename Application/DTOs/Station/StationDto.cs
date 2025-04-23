using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Station
{
    public class StationDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        public int CityId { get; set; }

        public string CityName { get; set; } = string.Empty;

        public int? CompanyId { get; set; }

        public string? CompanyName { get; set; }
    }
}
