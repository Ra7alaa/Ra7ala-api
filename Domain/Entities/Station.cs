using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class Station
    {
        public int Id { get; set; }

        [StringLength(30)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        [Required]

        public string Address { get; set; } = string.Empty;

        [StringLength(20)]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Required]
        public string Phone { get; set; } = string.Empty;

        [StringLength(20), EmailAddress , Required]

        public string Email { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        public int CityId { get; set; }

        public int? CompanyId { get; set; } = null;

        // Navigation properties
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; } = null!;




    }
}
