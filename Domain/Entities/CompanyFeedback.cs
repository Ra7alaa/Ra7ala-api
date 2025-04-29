using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    public class CompanyFeedback : BaseEntity<int>
    {
        public string PassengerId { get; set; } = string.Empty; // Changed to string for AppUser Id
        public int CompanyId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual AppUser Passenger { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
    }
}