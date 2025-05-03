using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket> GetTicketByCodeAsync(string ticketCode);
        Task<PaginatedTicketsResult> GetPaginatedTicketsByPassengerIdAsync(string passengerId, int pageNumber, int pageSize);
    }
    
    public class PaginatedTicketsResult
    {
        public int TotalCount { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}