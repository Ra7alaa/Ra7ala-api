using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class CityConfigurayion : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(c => c.Id);
            
            // Configure one-to-many relationship with Station
            builder.HasMany(c => c.Stations)
                   .WithOne(s => s.City)
                   .HasForeignKey(s => s.CityId)
                   .OnDelete(DeleteBehavior.Cascade);
            

        }
    }
}
