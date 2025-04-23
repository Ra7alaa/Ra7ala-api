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
    public class CompanyFeedback
    {
        public int Id { get; set; }
        public int PassengerId { get; set; }
        public int CompanyId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Passenger Passenger { get; set; }
        public virtual Company Company { get; set; }
       
    }
}