using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Company
{
    public class CompanyRatingsDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public List<FeedbackDto> Feedbacks { get; set; } = new();
    }
}