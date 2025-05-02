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
                   .HasForeignKey<Passenger>(p => p.Id)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship with Bookings
            builder.HasMany(p => p.Bookings)
                   .WithOne(b => b.Passenger)
                   .HasForeignKey(b => b.PassengerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many relationship with Tickets
            builder.HasMany(p => p.Tickets)
                   .WithOne(t => t.Passenger)
                   .HasForeignKey(t => t.PassengerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}