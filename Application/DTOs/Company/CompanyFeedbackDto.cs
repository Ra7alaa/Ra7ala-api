using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Company
{
    public class CompanyFeedbackDto
    {
        public int Id { get; set; }
        public string PassengerId { get; set; }
        public string PassengerName { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}