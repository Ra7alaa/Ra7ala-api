using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            // Configure relationship with Passenger
            builder.HasOne(b => b.Passenger)
                   .WithMany()
                   .HasForeignKey(b => b.PassengerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Trip
            builder.HasOne(b => b.Trip)
                   .WithMany(t => t.Bookings)
                   .HasForeignKey(b => b.TripId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Tickets
            builder.HasMany(b => b.Tickets)
                   .WithOne(t => t.Booking)
                   .HasForeignKey(t => t.BookingId)
                   .OnDelete(DeleteBehavior.Restrict); 

            // Configure properties
            builder.Property(b => b.BookingDate)
                   .IsRequired();

            builder.Property(b => b.TotalPrice)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Status)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}