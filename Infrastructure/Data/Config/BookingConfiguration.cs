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

            builder.Property(b => b.BookingDate)
                   .IsRequired();

            builder.Property(b => b.TotalPrice)
                   .IsRequired()
                   .HasPrecision(18, 2); // e.g., 123456789.99

            builder.Property(b => b.Status)
                   .IsRequired()
                   .HasMaxLength(50); // Limit length for efficiency

            // Configure relationship with Passenger
            builder.HasOne(b => b.Passenger)
                   .WithMany(p => p.Bookings) // Assuming Passenger has a Bookings collection
                   .HasForeignKey(b => b.PassengerId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deleting Passenger if they have bookings

            // Configure relationship with Trip
            builder.HasOne(b => b.Trip)
                   .WithMany(t => t.Bookings)
                   .HasForeignKey(b => b.TripId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deleting Trip if it has bookings

            // Configure relationship with Tickets
            builder.HasMany(b => b.Tickets)
                   .WithOne(t => t.Booking)
                   .HasForeignKey(t => t.BookingId)
                   .OnDelete(DeleteBehavior.Cascade); // Delete Tickets if Booking is deleted

            // Add indexes for performance
            builder.HasIndex(b => b.PassengerId);
            builder.HasIndex(b => b.TripId);
            builder.HasIndex(b => b.BookingDate);
            builder.HasIndex(b => b.Status);
        }
    }
}