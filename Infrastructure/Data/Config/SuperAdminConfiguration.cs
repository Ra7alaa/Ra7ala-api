using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class SuperAdminConfiguration : IEntityTypeConfiguration<SuperAdmin>
    {
        public void Configure(EntityTypeBuilder<SuperAdmin> builder)
        {
            // Configure one-to-one relationship with AppUser
            builder.HasOne(sa => sa.AppUser)
                .WithOne(u => u.SuperAdmin)
                .HasForeignKey<SuperAdmin>(sa => sa.Id);

            // Company relationship
            builder.HasOne(sa => sa.Company)
                .WithOne(c => c.SuperAdmin)
                .HasForeignKey<SuperAdmin>(sa => sa.CompanyId);
        }
    }
}