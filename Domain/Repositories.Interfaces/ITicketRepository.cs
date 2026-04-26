using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket?> GetTicketByCodeAsync(string ticketCode);
        Task<PaginatedTicketsResult> GetPaginatedTicketsByPassengerIdAsync(string passengerId, int pageNumber, int pageSize);
        /// <summary>Returns the highest assigned SeatNumber for the given trip, or null when none exist yet.</summary>
        Task<int?> GetMaxSeatNumberForTripAsync(int tripId);
    }
    
    public class PaginatedTicketsResult
    {
        public int TotalCount { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}