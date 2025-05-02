using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Route : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public int StartCityId { get; set; }
        public int EndCityId { get; set; }
        public int Distance { get; set; }
        public int EstimatedDuration { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual City StartCity { get; set; } = null!;
        public virtual City EndCity { get; set; } = null!;
        public virtual ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>(); 
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>(); 
    }
}