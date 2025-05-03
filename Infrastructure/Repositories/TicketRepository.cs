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

        public async Task<PaginatedTicketsResult> GetPaginatedTicketsByPassengerIdAsync(string passengerId, int pageNumber, int pageSize)
        {
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
    }
}