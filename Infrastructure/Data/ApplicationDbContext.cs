using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
     
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<SuperAdmin> SuperAdmins { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Driver> Drivers { get; set; } = null!;
        public DbSet<Passenger> Passengers { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Station> Stations { get; set; } = null!;
        public DbSet<Bus> Buses { get; set; } = null!; 
        public DbSet<Trip> Trips { get; set; } = null!; 
        public DbSet<TripStation> TripStations { get; set; } = null!;
        public DbSet<Route> Routes { get; set; } = null!;  
        public DbSet<RouteStation> RouteStations { get; set; } = null!;
        public DbSet<CompanyFeedback> CompanyFeedbacks { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}