using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class CompanyFeedbackConfiguration : IEntityTypeConfiguration<CompanyFeedback>
    {
        public void Configure(EntityTypeBuilder<CompanyFeedback> builder)
        {
            builder.HasKey(cf => cf.Id);

            builder.HasOne(cf => cf.Company)
                   .WithMany(c => c.Feedbacks)
                   .HasForeignKey(cf => cf.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cf => cf.Passenger)
                   .WithMany()
                   .HasForeignKey(cf => cf.PassengerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}