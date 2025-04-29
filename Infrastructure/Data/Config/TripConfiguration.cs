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
        }
    }
}