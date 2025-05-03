using Application.DTOs.Booking;
using Application.DTOs.Ticket;
using Application.Models;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Booking
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookingService> _logger;
        private readonly Random _random;

        public BookingService(IUnitOfWork unitOfWork, ILogger<BookingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _random = new Random();
        }

        // Create a new booking for a passenger
        public async Task<ServiceResult<BookingDto>> CreateBookingAsync(CreateBookingDto createDto, string passengerId)
        {
            try
            {
                // 1. Validate inputs
                if (createDto.StartStationId == createDto.EndStationId)
                    return ServiceResult<BookingDto>.Failure("Start and end stations cannot be the same");

                if (createDto.NumberOfTickets <= 0)
                    return ServiceResult<BookingDto>.Failure("Number of tickets must be greater than zero");

                // 2. Check if the trip exists
                var trip = await _unitOfWork.Trips.GetTripByIdWithDetailsAsync(createDto.TripId);
                if (trip == null)
                    return ServiceResult<BookingDto>.Failure($"Trip with ID {createDto.TripId} not found");

                // 3. Check if the trip has enough available seats
                if (trip.AvailableSeats < createDto.NumberOfTickets)
                    return ServiceResult<BookingDto>.Failure($"Not enough seats available. Available: {trip.AvailableSeats}, Requested: {createDto.NumberOfTickets}");

                // 4. Verify that start and end stations belong to this trip
                var tripStations = trip.TripStations.OrderBy(ts => ts.SequenceNumber).ToList();
                var startStation = tripStations.FirstOrDefault(ts => ts.StationId == createDto.StartStationId);
                var endStation = tripStations.FirstOrDefault(ts => ts.StationId == createDto.EndStationId);

                if (startStation == null)
                    return ServiceResult<BookingDto>.Failure($"Start station with ID {createDto.StartStationId} is not part of this trip");

                if (endStation == null)
                    return ServiceResult<BookingDto>.Failure($"End station with ID {createDto.EndStationId} is not part of this trip");

                if (startStation.SequenceNumber >= endStation.SequenceNumber)
                    return ServiceResult<BookingDto>.Failure("Start station must come before end station in the trip route");

                // 5. Get passenger
                var passenger = await _unitOfWork.Users.GetPassengerByUserIdAsync(passengerId);
                if (passenger == null)
                    return ServiceResult<BookingDto>.Failure($"Passenger with ID {passengerId} not found");

                // 6. Calculate total price (for the segment)
                decimal totalPrice = trip.Price * createDto.NumberOfTickets;

                // 7. Create booking
                var booking = new Domain.Entities.Booking
                {
                    PassengerId = passengerId,
                    TripId = createDto.TripId,
                    StartStationId = createDto.StartStationId,
                    EndStationId = createDto.EndStationId,
                    BookingDate = DateTime.UtcNow,
                    TotalPrice = totalPrice,
                    Status = BookingStatus.Pending,
                    IsPaid = false,
                    NumberOfTickets = createDto.NumberOfTickets
                };

                // 8. Save booking
                await _unitOfWork.Bookings.AddAsync(booking);

                // 9. Update available seats on trip
                trip.AvailableSeats -= createDto.NumberOfTickets;
                _unitOfWork.Trips.Update(trip);

                await _unitOfWork.SaveChangesAsync();

                // 10. Get full booking with details
                var createdBooking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(booking.Id);
                var bookingDto = MapBookingToDto(createdBooking);

                return ServiceResult<BookingDto>.Success(bookingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return ServiceResult<BookingDto>.Failure($"Error creating booking: {ex.Message}");
            }
        }

        // Get booking by ID
        public async Task<ServiceResult<BookingDto>> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
                if (booking == null)
                    return ServiceResult<BookingDto>.Failure($"Booking with ID {bookingId} not found");

                var bookingDto = MapBookingToDto(booking);
                return ServiceResult<BookingDto>.Success(bookingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving booking with ID {bookingId}");
                return ServiceResult<BookingDto>.Failure($"Error retrieving booking: {ex.Message}");
            }
        }

        // Get all bookings for a passenger
        public async Task<ServiceResult<PassengerBookingsResponseDto>> GetPassengerBookingsAsync(string passengerId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var paginatedBookings = await _unitOfWork.Bookings.GetPaginatedBookingsByPassengerIdAsync(passengerId, pageNumber, pageSize);
                
                var response = new PassengerBookingsResponseDto
                {
                    TotalCount = paginatedBookings.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Bookings = paginatedBookings.Bookings.Select(MapBookingToDto).ToList()
                };

                return ServiceResult<PassengerBookingsResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving bookings for passenger {passengerId}");
                return ServiceResult<PassengerBookingsResponseDto>.Failure($"Error retrieving bookings: {ex.Message}");
            }
        }

        // Process payment for a booking
        public async Task<ServiceResult<BookingConfirmationDto>> ProcessPaymentAsync(ProcessPaymentDto paymentDto, string passengerId)
        {
            try
            {
                // 1. Get the booking
                var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(paymentDto.BookingId);
                if (booking == null)
                    return ServiceResult<BookingConfirmationDto>.Failure($"Booking with ID {paymentDto.BookingId} not found");

                // 2. Verify the passenger owns this booking
                if (booking.PassengerId != passengerId)
                    return ServiceResult<BookingConfirmationDto>.Failure("You do not have permission to process this booking");

                // 3. Check if booking is already paid
                if (booking.IsPaid)
                    return ServiceResult<BookingConfirmationDto>.Failure("This booking has already been paid");

                // 4. Check booking status
                if (booking.Status != BookingStatus.Pending)
                    return ServiceResult<BookingConfirmationDto>.Failure($"Booking status is {booking.Status}, cannot process payment");

                // 5. Process payment (in a real application, you would integrate with a payment gateway)
                // Simulating payment processing...
                // Assume payment is successful

                // 6. Update booking status
                booking.Status = BookingStatus.Confirmed;
                booking.IsPaid = true;
                _unitOfWork.Bookings.Update(booking);

                // 7. Generate tickets
                var tickets = new List<Ticket>();
                for (int i = 0; i < booking.NumberOfTickets; i++)
                {
                    tickets.Add(new Ticket
                    {
                        TripId = booking.TripId,
                        BookingId = booking.Id,
                        PassengerId = booking.PassengerId,
                        Price = booking.TotalPrice / booking.NumberOfTickets, // Distribute price evenly
                        PurchaseDate = DateTime.UtcNow,
                        IsUsed = false,
                        TicketCode = GenerateRandomTicketCode()
                    });
                }

                // 8. Save tickets
                await _unitOfWork.Tickets.AddRangeAsync(tickets);
                await _unitOfWork.SaveChangesAsync();

                // 9. Prepare response
                var ticketDtos = tickets.Select(MapTicketToDto).ToList();

                var confirmation = new BookingConfirmationDto
                {
                    BookingId = booking.Id,
                    Success = true,
                    Message = "Payment processed successfully. Tickets have been generated.",
                    Tickets = ticketDtos
                };

                return ServiceResult<BookingConfirmationDto>.Success(confirmation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment for booking {paymentDto.BookingId}");
                return ServiceResult<BookingConfirmationDto>.Failure($"Error processing payment: {ex.Message}");
            }
        }

        // Cancel booking
        public async Task<ServiceResult> CancelBookingAsync(int bookingId, string passengerId)
        {
            try
            {
                // 1. Get the booking
                var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
                if (booking == null)
                    return ServiceResult.Failure($"Booking with ID {bookingId} not found");

                // 2. Verify the passenger owns this booking
                if (booking.PassengerId != passengerId)
                    return ServiceResult.Failure("You do not have permission to cancel this booking");

                // 3. Check if booking can be canceled
                if (booking.Status == BookingStatus.Cancelled)
                    return ServiceResult.Failure("This booking is already cancelled");

                // 4. If tickets are already issued and used, cannot cancel
                if (booking.IsPaid && booking.Tickets.Any(t => t.IsUsed))
                    return ServiceResult.Failure("Cannot cancel booking after tickets have been used");
                
                // 5. Update booking status
                booking.Status = BookingStatus.Cancelled;
                _unitOfWork.Bookings.Update(booking);
                
                // 6. Return seats to trip's available seats
                var trip = await _unitOfWork.Trips.GetByIdAsync(booking.TripId);
                if (trip != null)
                {
                    trip.AvailableSeats += booking.NumberOfTickets;
                    _unitOfWork.Trips.Update(trip);
                }
                
                await _unitOfWork.SaveChangesAsync();
                
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling booking {bookingId}");
                return ServiceResult.Failure($"Error cancelling booking: {ex.Message}");
            }
        }

        // Get tickets for a passenger
        public async Task<ServiceResult<PassengerTicketsResponseDto>> GetPassengerTicketsAsync(string passengerId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var paginatedTickets = await _unitOfWork.Tickets.GetPaginatedTicketsByPassengerIdAsync(passengerId, pageNumber, pageSize);
                
                var response = new PassengerTicketsResponseDto
                {
                    TotalCount = paginatedTickets.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Tickets = paginatedTickets.Tickets.Select(MapTicketToDto).ToList()
                };

                return ServiceResult<PassengerTicketsResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving tickets for passenger {passengerId}");
                return ServiceResult<PassengerTicketsResponseDto>.Failure($"Error retrieving tickets: {ex.Message}");
            }
        }

        // Helper methods
        private BookingDto MapBookingToDto(Domain.Entities.Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                PassengerId = booking.PassengerId,
                PassengerName = booking.Passenger?.AppUser?.FullName ?? string.Empty,
                TripId = booking.TripId,
                StartStationId = booking.StartStationId,
                StartStationName = booking.StartStation?.Name ?? string.Empty,
                StartCityId = booking.StartStation?.CityId ?? 0,
                StartCityName = booking.StartStation?.City?.Name ?? string.Empty,
                EndStationId = booking.EndStationId,
                EndStationName = booking.EndStation?.Name ?? string.Empty,
                EndCityId = booking.EndStation?.CityId ?? 0,
                EndCityName = booking.EndStation?.City?.Name ?? string.Empty,
                BookingDate = booking.BookingDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                IsPaid = booking.IsPaid,
                NumberOfTickets = booking.NumberOfTickets,
                Tickets = booking.Tickets?.Select(MapTicketToDto).ToList() ?? new List<TicketDto>()
            };
        }

        private TicketDto MapTicketToDto(Ticket ticket)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                TripId = ticket.TripId,
                BookingId = ticket.BookingId,
                PassengerId = ticket.PassengerId,
                PassengerName = ticket.Passenger?.AppUser?.FullName ?? string.Empty,
                SeatNumber = ticket.SeatNumber,
                Price = ticket.Price,
                PurchaseDate = ticket.PurchaseDate,
                IsUsed = ticket.IsUsed,
                TicketCode = ticket.TicketCode
            };
        }

        private string GenerateRandomTicketCode()
        {
            // Generate a random 4-digit code
            return _random.Next(1000, 10000).ToString();
        }
    }
}