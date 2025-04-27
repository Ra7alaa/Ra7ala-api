using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.DTOs.Company
{
    public class CompanyFilterDto
    {
        // public CompanyStatus? Status { get; set; }
        public string? Status { get; set; } = CompanyStatus.Pending.ToString();
        public string? SearchTerm { get; set; }
    }
}