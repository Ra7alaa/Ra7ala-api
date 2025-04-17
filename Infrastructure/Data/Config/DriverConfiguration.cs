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
                .HasForeignKey<Driver>(d => d.Id);

            // Company relationship
            builder.HasOne(d => d.Company)
                .WithMany(c => c.Drivers)
                .HasForeignKey(d => d.CompanyId);
        }
    }
}