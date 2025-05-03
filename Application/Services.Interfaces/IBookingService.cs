using Application.DTOs.Booking;
using Application.DTOs.Ticket;
using Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IBookingService
    {
        // Create a new booking for a passenger
        Task<ServiceResult<BookingDto>> CreateBookingAsync(CreateBookingDto createDto, string passengerId);
        
        // Get booking by ID
        Task<ServiceResult<BookingDto>> GetBookingByIdAsync(int bookingId);
        
        // Get all bookings for a passenger
        Task<ServiceResult<PassengerBookingsResponseDto>> GetPassengerBookingsAsync(string passengerId, int pageNumber = 1, int pageSize = 10);
        
        // Process payment for a booking
        Task<ServiceResult<BookingConfirmationDto>> ProcessPaymentAsync(ProcessPaymentDto paymentDto, string passengerId);
        
        // Cancel booking
        Task<ServiceResult> CancelBookingAsync(int bookingId, string passengerId);
        
        // Get tickets for a passenger
        Task<ServiceResult<PassengerTicketsResponseDto>> GetPassengerTicketsAsync(string passengerId, int pageNumber = 1, int pageSize = 10);
    }
}