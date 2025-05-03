using Application.DTOs.Booking;
using Application.DTOs.Ticket;
using Application.Models;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Errors;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        /// <param name="createBookingDto">The booking details</param>
        /// <returns>The created booking</returns>
        [Authorize(Roles = "Passenger")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateBooking(CreateBookingDto createBookingDto)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.CreateBookingAsync(createBookingDto, passengerId);
            
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Booking created successfully") { Data = result.Data });
                
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to create booking"));
        }

        /// <summary>
        /// Get a booking by ID
        /// </summary>
        /// <param name="id">The booking ID</param>
        /// <returns>The booking details</returns>
        [Authorize(Roles = "Passenger")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetBooking(int id)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.GetBookingByIdAsync(id);
            
            if (!result.IsSuccess)
                return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to retrieve booking"));
                
            if (result.Data.PassengerId != passengerId)
                return Forbid();
                
            return Ok(new ApiResponse(200, "Booking retrieved successfully") { Data = result.Data });
        }

        /// <summary>
        /// Get all bookings for the authenticated passenger
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of bookings</returns>
        [Authorize(Roles = "Passenger")]
        [HttpGet("my-bookings")]
        public async Task<ActionResult<ApiResponse>> GetMyBookings(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.GetPassengerBookingsAsync(passengerId, pageNumber, pageSize);
            
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Bookings retrieved successfully") { Data = result.Data });
                
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to retrieve bookings"));
        }

        /// <summary>
        /// Process payment for a booking
        /// </summary>
        /// <param name="paymentDto">Payment details</param>
        /// <returns>Booking confirmation with generated tickets</returns>
        [Authorize(Roles = "Passenger")]
        [HttpPost("payment")]
        public async Task<ActionResult<ApiResponse>> ProcessPayment(ProcessPaymentDto paymentDto)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.ProcessPaymentAsync(paymentDto, passengerId);
            
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Payment processed successfully") { Data = result.Data });
                
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Payment processing failed"));
        }

        /// <summary>
        /// Cancel a booking
        /// </summary>
        /// <param name="id">The booking ID to cancel</param>
        /// <returns>Success or failure message</returns>
        [Authorize(Roles = "Passenger")]
        [HttpPost("cancel/{id}")]
        public async Task<ActionResult<ApiResponse>> CancelBooking(int id)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.CancelBookingAsync(id, passengerId);
            
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Booking cancelled successfully"));
                
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to cancel booking"));
        }

        /// <summary>
        /// Get all tickets for the authenticated passenger
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of tickets</returns>
        [Authorize(Roles = "Passenger")]
        [HttpGet("my-tickets")]
        public async Task<ActionResult<ApiResponse>> GetMyTickets(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.GetPassengerTicketsAsync(passengerId, pageNumber, pageSize);
            
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Tickets retrieved successfully") { Data = result.Data });
                
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to retrieve tickets"));
        }
    }
}