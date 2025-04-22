using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public ICollection<Station> Stations { get; set; } = new List<Station>();
    }
}
