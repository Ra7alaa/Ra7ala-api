using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Company
{
    public class ReviewCompanyDto
    {
        public int CompanyId { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }

    }
}