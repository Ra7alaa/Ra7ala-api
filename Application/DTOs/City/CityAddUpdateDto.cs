using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}