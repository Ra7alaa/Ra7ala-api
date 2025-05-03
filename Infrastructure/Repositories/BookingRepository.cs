using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Booking?> GetBookingWithDetailsAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Passenger)
                    .ThenInclude(p => p!.AppUser)
                .Include(b => b.Trip)
                .Include(b => b.StartStation)
                    .ThenInclude(s => s!.City)
                .Include(b => b.EndStation)
                    .ThenInclude(s => s!.City)
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted);
        }

        public async Task<PaginatedBookingsResult> GetPaginatedBookingsByPassengerIdAsync(string passengerId, int pageNumber, int pageSize)
        {
            var query = _context.Bookings
                .Include(b => b.Passenger)
                    .ThenInclude(p => p!.AppUser)
                .Include(b => b.Trip)
                .Include(b => b.StartStation)
                    .ThenInclude(s => s!.City)
                .Include(b => b.EndStation)
                    .ThenInclude(s => s!.City)
                .Include(b => b.Tickets)
                .Where(b => b.PassengerId == passengerId && !b.IsDeleted)
                .OrderByDescending(b => b.BookingDate);

            var totalCount = await query.CountAsync();
            
            var bookings = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedBookingsResult
            {
                TotalCount = totalCount,
                Bookings = bookings
            };
        }
    }
}