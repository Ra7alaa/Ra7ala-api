using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.Id);

            // Configure relationship with Route
            builder.HasOne(t => t.Route)
                   .WithMany(r => r.Trips)
                   .HasForeignKey(t => t.RouteId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Driver
            builder.HasOne(t => t.Driver)
                   .WithMany(d => d.Trips)
                   .HasForeignKey(t => t.DriverId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Company
            builder.HasOne(t => t.Company)
                   .WithMany(c => c.Trips)
                   .HasForeignKey(t => t.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Bus
            builder.HasOne(t => t.Bus)
                   .WithMany()
                   .HasForeignKey(t => t.BusId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Tickets
            builder.HasMany(t => t.Tickets)
                   .WithOne(t => t.Trip)
                   .HasForeignKey(t => t.TripId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Bookings
            builder.HasMany(t => t.Bookings)
                   .WithOne(b => b.Trip)
                   .HasForeignKey(b => b.TripId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with TripStations
            builder.HasMany(t => t.TripStations)
                   .WithOne(ts => ts.Trip)
                   .HasForeignKey(ts => ts.TripId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure properties
            builder.Property(t => t.AvailableSeats)
                   .IsRequired();

            builder.Property(t => t.DepartureTime)
                   .IsRequired();

            builder.Property(t => t.IsCompleted)
                   .IsRequired();

            builder.Property(t => t.Price)
                   .IsRequired()
                   .HasPrecision(18, 2); // Added Price configuration

            // Add indexes for performance
            builder.HasIndex(t => t.RouteId);
            builder.HasIndex(t => t.CompanyId);
            builder.HasIndex(t => t.DepartureTime);
            builder.HasIndex(t => t.Price); // Added index for Price
        }
    }
}