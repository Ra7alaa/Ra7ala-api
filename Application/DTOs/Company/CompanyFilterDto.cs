using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Company
{
    public class CompanyFilterDto
    {
        public bool? IsApproved { get; set; }
        public bool? IsRejected { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SearchTerm { get; set; }
    }
}