using Domain.Entities;
using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Ticket?> GetTicketByCodeAsync(string ticketCode)
        {
            return await _context.Tickets
                .Include(t => t.Passenger)
                    .ThenInclude(p => p!.AppUser)
                .Include(t => t.Trip)
                .Include(t => t.Booking)
                .FirstOrDefaultAsync(t => t.TicketCode == ticketCode && !t.IsDeleted);
        }

        public async Task<PaginatedTicketsResult> GetPaginatedTicketsByPassengerIdAsync(
            string passengerId, int pageNumber, int pageSize)
        {
            // Use the explicit PassengerId column � not the EF shadow "PassengerId1"
            var query = _context.Tickets
                .Include(t => t.Passenger)
                    .ThenInclude(p => p!.AppUser)
                .Include(t => t.Trip)
                .Include(t => t.Booking)
                .Where(t => t.PassengerId == passengerId && !t.IsDeleted)
                .OrderByDescending(t => t.PurchaseDate);

            var totalCount = await query.CountAsync();

            var tickets = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedTicketsResult
            {
                TotalCount = totalCount,
                Tickets = tickets
            };
        }

        /// <inheritdoc />
        public async Task<int?> GetMaxSeatNumberForTripAsync(int tripId)
        {
            return await _context.Tickets
                .Where(t => t.TripId == tripId && t.SeatNumber != null && !t.IsDeleted)
                .MaxAsync(t => (int?)t.SeatNumber);
        }
    }
}