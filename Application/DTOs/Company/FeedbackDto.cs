using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Company
{
    public class FeedbackDto
    {
        public int Id { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerEmail { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}