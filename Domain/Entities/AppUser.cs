using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppUser : IdentityUser
    {   
        public UserType UserType { get; set; }
        
        // Navigation properties
        public SuperAdmin? SuperAdmin { get; set; }
        public Admin? Admin { get; set; }
        public Driver? Driver { get; set; }
        public Passenger? Passenger { get; set; }
    }
}