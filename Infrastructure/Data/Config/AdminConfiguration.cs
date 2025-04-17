using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            // Configure one-to-one relationship with AppUser
            builder.HasOne(a => a.AppUser)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.Id);

            // Company relationship
            builder.HasOne(a => a.Company)
                .WithMany(c => c.Admins)
                .HasForeignKey(a => a.CompanyId);

            // Added by relationship (Admin -> SuperAdmin)
            builder.HasOne(a => a.AddedBy)
                .WithMany(sa => sa.AddedAdmins)
                .HasForeignKey(a => a.AddedById);
        }
    }
}