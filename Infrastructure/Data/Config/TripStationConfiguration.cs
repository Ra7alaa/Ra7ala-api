using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class TripStationConfiguration : IEntityTypeConfiguration<TripStation>
    {
        public void Configure(EntityTypeBuilder<TripStation> builder)
        {
            builder.HasKey(ts => ts.Id);

            // Configure relationship with Trip
            builder.HasOne(ts => ts.Trip)
                   .WithMany(t => t.TripStations)
                   .HasForeignKey(ts => ts.TripId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship with Station
            builder.HasOne(ts => ts.Station)
                   .WithMany()
                   .HasForeignKey(ts => ts.StationId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Create a unique constraint for trip-sequence combination
            builder.HasIndex(ts => new { ts.TripId, ts.SequenceNumber })
                   .IsUnique();

            // Configure properties
            builder.Property(ts => ts.ArrivalTime)
                   .IsRequired(false); // Allow null for first station

            builder.Property(ts => ts.DepartureTime)
                   .IsRequired();

            builder.Property(ts => ts.IsActive)
                   .IsRequired();

            // Add indexes for performance
            builder.HasIndex(ts => ts.TripId);
            builder.HasIndex(ts => ts.StationId);
            builder.HasIndex(ts => ts.DepartureTime);
        }
    }
}