using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.DataSeed
{
    public class Ra7alaDataSeed
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Ra7alaDataSeed> _logger;

        public Ra7alaDataSeed(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<Ra7alaDataSeed> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding process...");

                // Get passwords from configuration
                var seedPasswords = _configuration.GetSection("SeedUserPasswords");
                var ownerPassword = seedPasswords["Owner"] ?? "Owner123!";
                var superAdminPassword = seedPasswords["SuperAdmin"] ?? "SuperAdmin123!";
                var adminPassword = seedPasswords["Admin"] ?? "Admin123!";
                var driverPassword = seedPasswords["Driver"] ?? "Driver123!";
                var passengerPassword = seedPasswords["Passenger"] ?? "Passenger123!";

                // Seed data in sequence
                await SeedRolesAsync();
                await SeedOwnerAsync(ownerPassword);
                await SeedCompaniesWithSuperAdminsAsync(superAdminPassword);
                await SeedAdminsAsync(adminPassword);
                await SeedDriversAsync(driverPassword);
                await SeedPassengersAsync(passengerPassword);

                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database seeding process");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            _logger.LogInformation("Seeding roles...");
            
            var roles = new[] { "Owner", "SuperAdmin", "Admin", "Driver", "Passenger" };
            
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogInformation("Creating role: {RoleName}", role);
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task SeedOwnerAsync(string password)
        {
            _logger.LogInformation("Seeding system owner...");
            
            if (await _userManager.FindByNameAsync("systemowner") == null)
            {
                _logger.LogInformation("Creating system owner user");
                
                var owner = new AppUser
                {
                    UserName = "systemOwner",
                    Email = "owner@ra7ala.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+201271520551",
                    FullName = "Eslam Elsayed",
                    Address = "Sharqia, Egypt",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    UserType = UserType.Owner,
                    DateCreated = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(owner, password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("System owner created successfully");
                    await _userManager.AddToRoleAsync(owner, "Owner");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to create system owner: {Errors}", errors);
                }
            }
        }

        private async Task SeedCompaniesWithSuperAdminsAsync(string password)
        {
            _logger.LogInformation("Seeding companies and super admins...");
            
            if (!await _context.Companies.AnyAsync())
            {
                _logger.LogInformation("Creating 5 companies");
                
                
                // In SeedCompaniesWithSuperAdminsAsync method
var companies = new List<Company>
{
    new Company
    {
        Name = "Cairo Transit",
        Email = "info@cairotransit.com",
        Phone = "+20223456789",
        Address = "10 Ramses Square, Cairo",
        Description = "Leading transportation company in Cairo",
        LogoUrl = "cairo_transit_logo.png",
        IsApproved = true,
        IsRejected = false,
        RejectionReason = string.Empty,
        CreatedDate = DateTime.UtcNow.AddMonths(-6),
        ApprovedDate = DateTime.UtcNow.AddMonths(-6).AddDays(3),
        AverageRating = 4.5,
        TotalRatings = 150
    },
    new Company
    {
        Name = "Alexandria Rides",
        Email = "contact@alexrides.com",
        Phone = "+20345678912",
        Address = "15 Corniche Road, Alexandria",
        Description = "Premium transportation in Alexandria",
        LogoUrl = "alex_rides_logo.png",
        IsApproved = true,
        IsRejected = false,
        RejectionReason = string.Empty,
        CreatedDate = DateTime.UtcNow.AddMonths(-5),
        ApprovedDate = DateTime.UtcNow.AddMonths(-5).AddDays(2),
        AverageRating = 4.2,
        TotalRatings = 120
    },
    new Company
    {
        Name = "Delta Transport",
        Email = "info@deltatransport.com",
        Phone = "+20456789123",
        Address = "22 El-Nasr Street, Mansoura",
        Description = "Serving the Delta region with quality transportation",
        LogoUrl = "delta_transport_logo.png",
        IsApproved = true,
        IsRejected = false,
        RejectionReason = string.Empty,
        CreatedDate = DateTime.UtcNow.AddMonths(-4),
        ApprovedDate = DateTime.UtcNow.AddMonths(-4).AddDays(5),
        AverageRating = 4.0,
        TotalRatings = 90
    },
    new Company
    {
        Name = "Luxor Travels",
        Email = "support@luxortravels.com",
        Phone = "+20567891234",
        Address = "5 Karnak Temple Road, Luxor",
        Description = "Tourist transportation in Upper Egypt",
        LogoUrl = "luxor_travels_logo.png",
        IsApproved = true,
        IsRejected = false,
        RejectionReason = string.Empty,
        CreatedDate = DateTime.UtcNow.AddMonths(-3),
        ApprovedDate = DateTime.UtcNow.AddMonths(-3).AddDays(4),
        AverageRating = 4.8,
        TotalRatings = 200
    },
    new Company
    {
        Name = "Red Sea Mobility",
        Email = "info@redseamobility.com",
        Phone = "+20678912345",
        Address = "30 El-Salam Road, Hurghada",
        Description = "Transportation solutions in Red Sea resorts",
        LogoUrl = "red_sea_mobility_logo.png",
        IsApproved = true,
        IsRejected = false,
        RejectionReason = string.Empty,
        CreatedDate = DateTime.UtcNow.AddMonths(-2),
        ApprovedDate = DateTime.UtcNow.AddMonths(-2).AddDays(3),
        AverageRating = 4.6,
        TotalRatings = 180
    }
};

                await _context.Companies.AddRangeAsync(companies);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Creating 5 super admins for companies");
                
                var superAdminCredentials = new List<(string username, string email, string fullName, string phone, DateTime dob, int companyId)>
                {
                    ("karim_superadmin", "karim@cairotransit.com", "Karim Hassan", "+20123456789", new DateTime(1980, 3, 15), 1),
                    ("nour_superadmin", "nour@alexrides.com", "Nour Ahmed", "+20123456790", new DateTime(1979, 7, 22), 2),
                    ("amr_superadmin", "amr@deltatransport.com", "Amr Mahmoud", "+20123456791", new DateTime(1982, 5, 10), 3),
                    ("heba_superadmin", "heba@luxortravels.com", "Heba Ali", "+20123456792", new DateTime(1981, 9, 18), 4),
                    ("salem_superadmin", "salem@redseamobility.com", "Salem Mostafa", "+20123456793", new DateTime(1978, 11, 25), 5)
                };

                foreach (var cred in superAdminCredentials)
                {
                    if (await _userManager.FindByNameAsync(cred.username) == null)
                    {
                        _logger.LogInformation("Creating super admin user: {Username}", cred.username);
                        
                        var superAdmin = new AppUser
                        {
                            UserName = cred.username,
                            Email = cred.email,
                            EmailConfirmed = true,
                            PhoneNumber = cred.phone,
                            FullName = cred.fullName,
                            Address = await _context.Companies
                                .Where(c => c.Id == cred.companyId)
                                .Select(c => c.Address)
                                .FirstOrDefaultAsync() ?? string.Empty,
                            DateOfBirth = cred.dob,
                            UserType = UserType.SuperAdmin,
                            DateCreated = DateTime.UtcNow.AddMonths(-6)
                        };

                        var result = await _userManager.CreateAsync(superAdmin, password);
                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

                            // Create SuperAdmin entity
                            var superAdminEntity = new SuperAdmin
                            {
                                Id = superAdmin.Id,
                                CompanyId = cred.companyId,
                                AppUser = superAdmin
                            };

                            await _context.SuperAdmins.AddAsync(superAdminEntity);
                            await _context.SaveChangesAsync();

                            // Update Company with SuperAdmin
                            var company = await _context.Companies.FindAsync(cred.companyId);
                            if (company != null)
                            {
                                company.SuperAdmin = superAdminEntity;
                                await _context.SaveChangesAsync();
                            }
                            
                            _logger.LogInformation("Super admin {Username} created successfully", cred.username);
                        }
                        else
                        {
                            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                            _logger.LogWarning("Failed to create super admin {Username}: {Errors}", cred.username, errors);
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("Companies and super admins already exist, skipping seed");
            }
        }

        private async Task SeedAdminsAsync(string password)
        {
            _logger.LogInformation("Seeding admins...");
            
            var companies = await _context.Companies
                .Include(c => c.SuperAdmin)
                .ToListAsync();
            
            if (companies.Any() && !await _context.Admins.AnyAsync())
            {
                _logger.LogInformation("Creating 10 admins (2 per company)");
                
                var adminCredentials = new List<(string username, string email, string fullName, string phone, DateTime dob, int companyId, string department)>
                {
                    // Cairo Transit admins
                    ("mohamed_admin", "mohamed@cairotransit.com", "Mohamed Samir", "+20123456794", new DateTime(1988, 2, 20), 1, "Operations"),
                    ("fatma_admin", "fatma@cairotransit.com", "Fatma Mahmoud", "+20123456795", new DateTime(1990, 4, 15), 1, "Customer Support"),
                    
                    // Alexandria Rides admins
                    ("yousef_admin", "yousef@alexrides.com", "Yousef Khaled", "+20123456796", new DateTime(1985, 6, 10), 2, "Fleet Management"),
                    ("laila_admin", "laila@alexrides.com", "Laila Adel", "+20123456797", new DateTime(1989, 8, 5), 2, "HR"),
                    
                    // Delta Transport admins
                    ("tariq_admin", "tariq@deltatransport.com", "Tariq Essam", "+20123456798", new DateTime(1987, 10, 12), 3, "Operations"),
                    ("rana_admin", "rana@deltatransport.com", "Rana Gamal", "+20123456799", new DateTime(1991, 12, 8), 3, "Finance"),
                    
                    // Luxor Travels admins
                    ("hany_admin", "hany@luxortravels.com", "Hany Tamer", "+20123456800", new DateTime(1986, 1, 25), 4, "Tourism"),
                    ("dalia_admin", "dalia@luxortravels.com", "Dalia Fouad", "+20123456801", new DateTime(1992, 3, 18), 4, "Sales"),
                    
                    // Red Sea Mobility admins
                    ("fady_admin", "fady@redseamobility.com", "Fady Nabil", "+20123456802", new DateTime(1984, 5, 30), 5, "Resort Services"),
                    ("yasmin_admin", "yasmin@redseamobility.com", "Yasmin Omar", "+20123456803", new DateTime(1993, 7, 22), 5, "Customer Experience")
                };

                foreach (var cred in adminCredentials)
                {
                    if (await _userManager.FindByNameAsync(cred.username) == null)
                    {
                        _logger.LogInformation("Creating admin user: {Username}", cred.username);
                        
                        var company = companies.First(c => c.Id == cred.companyId);
                        var superAdminId = company.SuperAdmin?.Id;

                        if (!string.IsNullOrEmpty(superAdminId))
                        {
                            var admin = new AppUser
                            {
                                UserName = cred.username,
                                Email = cred.email,
                                EmailConfirmed = true,
                                PhoneNumber = cred.phone,
                                FullName = cred.fullName,
                                Address = company.Address,
                                DateOfBirth = cred.dob,
                                UserType = UserType.Admin,
                                DateCreated = DateTime.UtcNow.AddMonths(-5)
                            };

                            var result = await _userManager.CreateAsync(admin, password);
                            if (result.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(admin, "Admin");

                                // Create Admin entity
                                var adminEntity = new Admin
                                {
                                    Id = admin.Id,
                                    CompanyId = cred.companyId,
                                    Department = cred.department,
                                    AddedById = superAdminId,
                                    AppUser = admin
                                };

                                await _context.Admins.AddAsync(adminEntity);
                                await _context.SaveChangesAsync();
                                
                                _logger.LogInformation("Admin {Username} created successfully", cred.username);
                            }
                            else
                            {
                                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                                _logger.LogWarning("Failed to create admin {Username}: {Errors}", cred.username, errors);
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("Admins already exist or companies not found, skipping admin seed");
            }
        }

        private async Task SeedDriversAsync(string password)
        {
            _logger.LogInformation("Seeding drivers...");
            
            var companies = await _context.Companies.ToListAsync();
            
            if (companies.Any() && !await _context.Drivers.AnyAsync())
            {
                _logger.LogInformation("Creating 10 drivers (2 per company)");
                
                var driverCredentials = new List<(string username, string email, string fullName, string phone, DateTime dob, int companyId, string license, DateTime licenseExpiry)>
                {
                    // Cairo Transit drivers
                    ("ahmed_driver", "ahmed@cairotransit.com", "Ahmed Sayed", "+20123456804", new DateTime(1990, 8, 15), 1, "DL123456", DateTime.Now.AddYears(2)),
                    ("ibrahim_driver", "ibrahim@cairotransit.com", "Ibrahim Hassan", "+20123456805", new DateTime(1988, 9, 22), 1, "DL123457", DateTime.Now.AddYears(3)),
                    
                    // Alexandria Rides drivers
                    ("omar_driver", "omar@alexrides.com", "Omar Farag", "+20123456806", new DateTime(1992, 2, 10), 2, "DL123458", DateTime.Now.AddYears(1)),
                    ("ali_driver", "ali@alexrides.com", "Ali Kareem", "+20123456807", new DateTime(1991, 4, 5), 2, "DL123459", DateTime.Now.AddYears(2)),
                    
                    // Delta Transport drivers
                    ("mahmoud_driver", "mahmoud@deltatransport.com", "Mahmoud Samir", "+20123456808", new DateTime(1989, 6, 12), 3, "DL123460", DateTime.Now.AddYears(3)),
                    ("hassan_driver", "hassan@deltatransport.com", "Hassan Ahmed", "+20123456809", new DateTime(1993, 7, 8), 3, "DL123461", DateTime.Now.AddYears(1)),
                    
                    // Luxor Travels drivers
                    ("osama_driver", "osama@luxortravels.com", "Osama Maher", "+20123456810", new DateTime(1987, 10, 25), 4, "DL123462", DateTime.Now.AddYears(2)),
                    ("sherif_driver", "sherif@luxortravels.com", "Sherif Hamdy", "+20123456811", new DateTime(1994, 11, 18), 4, "DL123463", DateTime.Now.AddYears(3)),
                    
                    // Red Sea Mobility drivers
                    ("tamer_driver", "tamer@redseamobility.com", "Tamer Hany", "+20123456812", new DateTime(1986, 3, 30), 5, "DL123464", DateTime.Now.AddYears(1)),
                    ("waleed_driver", "waleed@redseamobility.com", "Waleed Magdy", "+20123456813", new DateTime(1992, 5, 22), 5, "DL123465", DateTime.Now.AddYears(2))
                };

                foreach (var cred in driverCredentials)
                {
                    if (await _userManager.FindByNameAsync(cred.username) == null)
                    {
                        _logger.LogInformation("Creating driver user: {Username}", cred.username);
                        
                        var company = companies.First(c => c.Id == cred.companyId);
                        
                        var driver = new AppUser
                        {
                            UserName = cred.username,
                            Email = cred.email,
                            EmailConfirmed = true,
                            PhoneNumber = cred.phone,
                            FullName = cred.fullName,
                            Address = company.Address,
                            DateOfBirth = cred.dob,
                            UserType = UserType.Driver,
                            DateCreated = DateTime.UtcNow.AddMonths(-4)
                        };

                        var result = await _userManager.CreateAsync(driver, password);
                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(driver, "Driver");

                            // Create Driver entity
                            var driverEntity = new Driver
                            {
                                Id = driver.Id,
                                CompanyId = cred.companyId,
                                LicenseNumber = cred.license,
                                LicenseExpiryDate = cred.licenseExpiry,
                                ContactAddress = $"{cred.fullName} Address, {company.Address}",
                                HireDate = DateTime.UtcNow.AddMonths(-4),
                                DriverStatus = DriverStatus.Active,
                                AppUser = driver
                            };

                            await _context.Drivers.AddAsync(driverEntity);
                            await _context.SaveChangesAsync();
                            
                            _logger.LogInformation("Driver {Username} created successfully", cred.username);
                        }
                        else
                        {
                            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                            _logger.LogWarning("Failed to create driver {Username}: {Errors}", cred.username, errors);
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("Drivers already exist or companies not found, skipping driver seed");
            }
        }

        private async Task SeedPassengersAsync(string password)
        {
            _logger.LogInformation("Seeding passengers...");
            
            if (!await _context.Passengers.AnyAsync())
            {
                _logger.LogInformation("Creating 5 passengers");
                
                var passengerCredentials = new List<(string username, string email, string fullName, string phone, DateTime dob, string address)>
                {
                    ("sara_passenger", "sara@example.com", "Sara Ahmed", "+20123456814", new DateTime(1995, 4, 12), "25 El-Tahrir Street, Cairo"),
                    ("mohamed_passenger", "mohamed.p@example.com", "Mohamed Ali", "+20123456815", new DateTime(1992, 7, 8), "10 El-Horreya Road, Alexandria"),
                    ("nada_passenger", "nada@example.com", "Nada Mahmoud", "+20123456816", new DateTime(1996, 9, 15), "7 El-Gomhoria Street, Mansoura"),
                    ("adel_passenger", "adel@example.com", "Adel Hussein", "+20123456817", new DateTime(1990, 11, 22), "15 Luxor-Aswan Road, Luxor"),
                    ("reem_passenger", "reem@example.com", "Reem Tarek", "+20123456818", new DateTime(1993, 2, 18), "20 El-Nasr Street, Hurghada")
                };

                foreach (var cred in passengerCredentials)
                {
                    if (await _userManager.FindByNameAsync(cred.username) == null)
                    {
                        _logger.LogInformation("Creating passenger user: {Username}", cred.username);
                        
                        var passenger = new AppUser
                        {
                            UserName = cred.username,
                            Email = cred.email,
                            EmailConfirmed = true,
                            PhoneNumber = cred.phone,
                            FullName = cred.fullName,
                            Address = cred.address,
                            DateOfBirth = cred.dob,
                            UserType = UserType.Passenger,
                            DateCreated = DateTime.UtcNow.AddMonths(-3)
                        };

                        var result = await _userManager.CreateAsync(passenger, password);
                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(passenger, "Passenger");

                            // Create Passenger entity
                            var passengerEntity = new Passenger
                            {
                                Id = passenger.Id,
                                AppUser = passenger
                            };

                            await _context.Passengers.AddAsync(passengerEntity);
                            await _context.SaveChangesAsync();
                            
                            _logger.LogInformation("Passenger {Username} created successfully", cred.username);
                        }
                        else
                        {
                            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                            _logger.LogWarning("Failed to create passenger {Username}: {Errors}", cred.username, errors);
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("Passengers already exist, skipping passenger seed");
            }
        }
    }
}