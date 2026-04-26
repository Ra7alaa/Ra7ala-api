using Application.DTOs.Booking;
using Application.Models;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Errors;
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

        [Authorize(Roles = "Passenger")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateBooking(CreateBookingDto createBookingDto)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(passengerId)) return Unauthorized(new ApiResponse(401, "User not identified"));
            
            var result = await _bookingService.CreateBookingAsync(createBookingDto, passengerId);
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Booking created successfully") { Data = result.Data });
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to create booking"));
        }

        [Authorize(Roles = "Passenger")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetBooking(int id)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(passengerId)) return Unauthorized(new ApiResponse(401, "User not identified"));

            var result = await _bookingService.GetBookingByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to retrieve booking"));
            
            if (result.Data?.PassengerId != passengerId)
                return Forbid();
            
            return Ok(new ApiResponse(200, "Booking retrieved successfully") { Data = result.Data });
        }

        [Authorize(Roles = "Passenger")]
        [HttpGet("my-bookings")]
        public async Task<ActionResult<ApiResponse>> GetMyBookings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(passengerId)) return Unauthorized(new ApiResponse(401, "User not identified"));

            var result = await _bookingService.GetPassengerBookingsAsync(passengerId, pageNumber, pageSize);
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Bookings retrieved successfully") { Data = result.Data });
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to retrieve bookings"));
        }

        [Authorize(Roles = "Passenger")]
        [HttpPost("payment")]
        public async Task<ActionResult<ApiResponse>> ProcessPayment(ProcessPaymentDto paymentDto)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(passengerId)) return Unauthorized(new ApiResponse(401, "User not identified"));

            var result = await _bookingService.ProcessPaymentAsync(paymentDto, passengerId);
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Payment processed successfully") { Data = result.Data });
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Payment processing failed"));
        }

        [Authorize(Roles = "Passenger")]
        [HttpPost("cancel/{id}")]
        public async Task<ActionResult<ApiResponse>> CancelBooking(int id)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(passengerId)) return Unauthorized(new ApiResponse(401, "User not identified"));

            var result = await _bookingService.CancelBookingAsync(id, passengerId);
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Booking cancelled successfully"));
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to cancel booking"));
        }

        [Authorize(Roles = "Passenger")]
        [HttpGet("my-tickets")]
        public async Task<ActionResult<ApiResponse>> GetMyTickets(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(passengerId)) return Unauthorized(new ApiResponse(401, "User not identified"));

            var result = await _bookingService.GetPassengerTicketsAsync(passengerId, pageNumber, pageSize);
            if (result.IsSuccess)
                return Ok(new ApiResponse(200, "Tickets retrieved successfully") { Data = result.Data });
            return BadRequest(new ApiResponse(400, result.Errors.FirstOrDefault() ?? "Failed to retrieve tickets"));
        }
    }
}
