using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.Id);

            // Configure relationship with Trip
            builder.HasOne(t => t.Trip)
                   .WithMany(t => t.Tickets)
                   .HasForeignKey(t => t.TripId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Booking (optional)
            builder.HasOne(t => t.Booking)
                   .WithMany(b => b.Tickets)
                   .HasForeignKey(t => t.BookingId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Passenger
            builder.HasOne(t => t.Passenger)
                   .WithMany()
                   .HasForeignKey(t => t.PassengerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Ensure SeatNumber is unique per Trip (if assigned)
            builder.HasIndex(t => new { t.TripId, t.SeatNumber })
                   .IsUnique()
                   .HasFilter("[SeatNumber] IS NOT NULL"); // Modification: Added to ensure unique seat numbers per trip

            // Configure properties
            builder.Property(t => t.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(t => t.PurchaseDate)
                   .IsRequired();

            builder.Property(t => t.IsUsed)
                   .IsRequired();
                   
            // Configure TicketCode property
            builder.Property(t => t.TicketCode)
                   .IsRequired()
                   .HasMaxLength(4);
                   
            // Add an index on TicketCode for faster lookups
            builder.HasIndex(t => t.TicketCode);
        }
    }
}