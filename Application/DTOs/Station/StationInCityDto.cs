using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Station
{
    public class StationInCityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        // يمكن إضافة CompanyName إذا كان مطلوبًا عرض اسم الشركة المالكة للمحطة
        public string? CompanyName { get; set; }
    }
}