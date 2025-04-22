using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.HasKey(s => s.Id);
            
            // Configure one-to-many relationship with City
            builder.HasOne(s => s.City)
                   .WithMany(c => c.Stations)
                   .HasForeignKey(s => s.CityId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
            
            // Configure one-to-many relationship with Company (optional)
            builder.HasOne(s => s.Company)
                   .WithMany()
                   .HasForeignKey(s => s.CompanyId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
            

        }
    }
}