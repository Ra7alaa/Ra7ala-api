using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Bus
{
    public class UpdateBusDto
    {
        [Required]
        [MaxLength(50)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Capacity { get; set; }

        [Required]
        public string AmenityDescription { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
