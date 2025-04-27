using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            // Configure one-to-one relationship with AppUser
            builder.HasOne(d => d.AppUser)
                   .WithOne(u => u.Driver)
                   .HasForeignKey<Driver>(d => d.Id)
                   .OnDelete(DeleteBehavior.Cascade);

            // Company relationship
            builder.HasOne(d => d.Company)
                   .WithMany(c => c.Drivers)
                   .HasForeignKey(d => d.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure license details
            builder.Property(d => d.LicenseNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            // Configure relation with trips
            builder.HasMany(d => d.Trips)
                   .WithOne(t => t.Driver)
                   .HasForeignKey(t => t.DriverId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}