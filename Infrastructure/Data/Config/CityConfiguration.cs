using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(50);
                   
            builder.Property(c => c.Governorate)
                   .IsRequired()
                   .HasMaxLength(50);
            
            // Configure one-to-many relationship with Station
            builder.HasMany(c => c.Stations)
                   .WithOne(s => s.City)
                   .HasForeignKey(s => s.CityId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            // Routes as starting city
            builder.HasMany<Route>()
                   .WithOne(r => r.StartCity)
                   .HasForeignKey(r => r.StartCityId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            // Routes as ending city
            builder.HasMany<Route>()
                   .WithOne(r => r.EndCity)
                   .HasForeignKey(r => r.EndCityId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
