using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
                .AsSplitQuery()
                .FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted);
        }

        public async Task<PaginatedBookingsResult> GetPaginatedBookingsByPassengerIdAsync(
            string passengerId, int pageNumber, int pageSize)
        {
            // Count without any includes — fast
            var totalCount = await _context.Bookings
                .Where(b => b.PassengerId == passengerId && !b.IsDeleted)
                .CountAsync();

            // AsSplitQuery: EF issues one SELECT per Include instead of
            // one giant Cartesian join — fixes "empty navigation" on multi-include queries
            var bookings = await _context.Bookings
                .Where(b => b.PassengerId == passengerId && !b.IsDeleted)
                .OrderByDescending(b => b.BookingDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(b => b.Passenger)
                    .ThenInclude(p => p!.AppUser)
                .Include(b => b.Trip)
                .Include(b => b.StartStation)
                    .ThenInclude(s => s!.City)
                .Include(b => b.EndStation)
                    .ThenInclude(s => s!.City)
                .Include(b => b.Tickets)
                .AsSplitQuery()
                .ToListAsync();

            return new PaginatedBookingsResult
            {
                TotalCount = totalCount,
                Bookings = bookings
            };
        }
    }
}