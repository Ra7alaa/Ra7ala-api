using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class StationRepository : GenericRepository<Station>, IStationRepository
    {
        public StationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Station>> GetStationsByCityIdAsync(int cityId)
        {
            return await _context.Stations
                .Include(s => s.City)
                .Include(s => s.Company)
                .Where(s => s.CityId == cityId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Station>> GetStationsByCompanyIdAsync(int companyId)
        {
            return await _context.Stations
                .Include(s => s.City)
                .Include(s => s.Company)
                .Where(s => s.CompanyId == companyId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Station>> GetNearbyStationsAsync(double latitude, double longitude, double radiusInKm)
        {
            // استرجاع جميع المحطات
            var stations = await _context.Stations
                .Include(s => s.City)
                .Include(s => s.Company)
                .Where(s => !s.IsDeleted)
                .ToListAsync();

            // حساب المسافة بين الموقع المعطى وكل محطة
            var nearbyStations = stations.Where(s => 
            {
                // تحويل الإحداثيات من decimal إلى double لحساب المسافة
                double stationLat = (double)s.Latitude;
                double stationLng = (double)s.Longitude;
                var distance = CalculateDistance(latitude, longitude, stationLat, stationLng);
                return distance <= radiusInKm;
            });

            return nearbyStations;
        }

        // حساب المسافة بين نقطتين جغرافيتين باستخدام صيغة هافرسين
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0; // نصف قطر الأرض بالكيلومتر

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = EarthRadiusKm * c;

            return distance;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        // تجاوز الطرق الأساسية للحصول على محطات مع البيانات المرتبطة
        public new async Task<IEnumerable<Station>> GetAllAsync()
        {
            return await _context.Stations
                .Include(s => s.City)
                .Include(s => s.Company)
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }

        // الحصول على محطات النظام فقط (CompanyId = null)
        public async Task<IEnumerable<Station>> GetSystemStationsAsync()
        {
            return await _context.Stations
                .Include(s => s.City)
                .Include(s => s.Company)
                .Where(s => s.CompanyId == null && !s.IsDeleted)
                .ToListAsync();
        }

        // تعديل الدالة لتأخذ معرف الشركة كمعامل
        public async Task<IEnumerable<Station>> GetCompanyStationsAsync(int companyId)
        {
            return await _context.Stations
                .Include(s => s.City)
                .Include(s => s.Company)
                .Where(s => s.CompanyId == companyId && !s.IsDeleted)
                .ToListAsync();
        }

        // دالة جديدة للحصول على محطات النظام مع محطات شركة محددة
        public async Task<IEnumerable<Station>> GetSystemAndCompanyStationsAsync(int companyId)
        {
            return await _context.Stations
                .Include(s => s.City)
                .Include(s => s.Company)
                .Where(s => (s.CompanyId == null || s.CompanyId == companyId) && !s.IsDeleted)
                .ToListAsync();
        }

        public new async Task<Station?> GetByIdAsync(int id)
        {
            return await _context.Stations
                .Include(s => s.City)
                .Include(s => s.Company)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<IEnumerable<Station>> GetStationsByCityNameAsync(string cityName)
        {
            return await _context.Stations
                .Include(s => s.City)
                .Include(s => s.Company)
                .Where(s => s.City.Name == cityName && !s.IsDeleted)
                .ToListAsync();
        }
    }
}