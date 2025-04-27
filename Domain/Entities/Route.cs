using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StartCityId { get; set; }
        public int EndCityId { get; set; }
        public int Distance { get; set; } 
        public int EstimatedDuration { get; set; } 
        public int CompanyId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual City StartCity { get; set; }
        public virtual City EndCity { get; set; }
        public virtual ICollection<RouteStation> RouteStations { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}