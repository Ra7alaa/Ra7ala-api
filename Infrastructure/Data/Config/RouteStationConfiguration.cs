using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class RouteStationConfiguration : IEntityTypeConfiguration<RouteStation>
    {
        public void Configure(EntityTypeBuilder<RouteStation> builder)
        {
            builder.HasKey(rs => rs.Id);

            // Configure relationship with Route
            builder.HasOne(rs => rs.Route)
                   .WithMany(r => r.RouteStations)
                   .HasForeignKey(rs => rs.RouteId)
                   .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cascade delete issues

            // Configure relationship with Station
            builder.HasOne(rs => rs.Station)
                   .WithMany()
                   .HasForeignKey(rs => rs.StationId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Create a unique constraint for route-station-sequence combination
            builder.HasIndex(rs => new { rs.RouteId, rs.StationId })
                   .IsUnique();
            
            builder.HasIndex(rs => new { rs.RouteId, rs.SequenceNumber })
                   .IsUnique();
        }
    }
}