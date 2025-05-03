using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            // Primary key
            builder.HasKey(b => b.Id);

            // Configure properties
            builder.Property(b => b.PassengerId)
                   .IsRequired();

            builder.Property(b => b.TripId)
                   .IsRequired();
                   
            builder.Property(b => b.StartStationId)
                   .IsRequired();
                   
            builder.Property(b => b.EndStationId)
                   .IsRequired();

            builder.Property(b => b.BookingDate)
                   .IsRequired();

            builder.Property(b => b.TotalPrice)
                   .IsRequired()
                   .HasPrecision(18, 2); // e.g., 123456789.99

            builder.Property(b => b.Status)
                   .IsRequired();

            builder.Property(b => b.IsPaid)
                   .IsRequired()
                   .HasDefaultValue(false);
                   
            builder.Property(b => b.NumberOfTickets)
                   .IsRequired()
                   .HasDefaultValue(1);

            // Configure relationship with Passenger
            builder.HasOne(b => b.Passenger)
                   .WithMany(p => p.Bookings)
                   .HasForeignKey(b => b.PassengerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Trip
            builder.HasOne(b => b.Trip)
                   .WithMany(t => t.Bookings)
                   .HasForeignKey(b => b.TripId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            // Configure relationship with StartStation
            builder.HasOne(b => b.StartStation)
                   .WithMany()
                   .HasForeignKey(b => b.StartStationId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            // Configure relationship with EndStation
            builder.HasOne(b => b.EndStation)
                   .WithMany()
                   .HasForeignKey(b => b.EndStationId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Tickets
            builder.HasMany(b => b.Tickets)
                   .WithOne(t => t.Booking)
                   .HasForeignKey(t => t.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for performance
            builder.HasIndex(b => b.PassengerId);
            builder.HasIndex(b => b.TripId);
            builder.HasIndex(b => b.StartStationId);
            builder.HasIndex(b => b.EndStationId);
            builder.HasIndex(b => b.BookingDate);
            builder.HasIndex(b => b.Status);
        }
    }
}