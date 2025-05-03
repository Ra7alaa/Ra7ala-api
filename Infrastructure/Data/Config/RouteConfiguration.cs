using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Configure relationship with Company
            builder.HasOne(r => r.Company)
                   .WithMany(c => c.Routes)
                   .HasForeignKey(r => r.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with StartCity
            builder.HasOne(r => r.StartCity)
                   .WithMany()
                   .HasForeignKey(r => r.StartCityId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with EndCity
            builder.HasOne(r => r.EndCity)
                   .WithMany()
                   .HasForeignKey(r => r.EndCityId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many relationship with RouteStation
            builder.HasMany(r => r.RouteStations)
                   .WithOne(rs => rs.Route)
                   .HasForeignKey(rs => rs.RouteId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship with Trip
            builder.HasMany(r => r.Trips)
                   .WithOne(t => t.Route)
                   .HasForeignKey(t => t.RouteId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Add indexes for performance
            builder.HasIndex(r => r.CompanyId);
            builder.HasIndex(r => r.Name);
        }
    }
}