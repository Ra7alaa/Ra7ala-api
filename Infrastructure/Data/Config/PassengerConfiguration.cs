using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            // Configure one-to-one relationship with AppUser
            builder.HasOne(p => p.AppUser)
                .WithOne(u => u.Passenger)
                .HasForeignKey<Passenger>(p => p.Id);
        }
    }
}