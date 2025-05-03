using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<Booking> GetBookingWithDetailsAsync(int bookingId);
        Task<PaginatedBookingsResult> GetPaginatedBookingsByPassengerIdAsync(string passengerId, int pageNumber, int pageSize);
    }
    
    public class PaginatedBookingsResult
    {
        public int TotalCount { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}