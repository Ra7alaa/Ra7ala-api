using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        await SeedCitiesAsync();
        await SeedStationsAsync();
        
        // Add new seeding methods
        await SeedBusesAsync();
        // In SeedAsync method
        await SeedRoutesAsync();
        await SeedRouteStationsAsync();  
        await SeedTripsAsync();
        await SeedTripStationsAsync();  
      
        //await SeedBookingsAndTicketsAsync();

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
                        Status = "Approved",
                        RejectionReason = string.Empty,
                        CreatedDate = DateTime.UtcNow.AddMonths(-6),
                        ApprovedDate = DateTime.UtcNow.AddMonths(-6).AddDays(3),
                        AverageRating = 0,
                        TotalRatings = 0,
                        SuperAdminName = "Karim Hassan",
                        SuperAdminEmail = "karim@cairotransit.com",
                        SuperAdminPhone = "+20123456789"
                    },
                    new Company
                    {
                        Name = "Alexandria Rides",
                        Email = "contact@alexrides.com",
                        Phone = "+20345678912",
                        Address = "15 Corniche Road, Alexandria",
                        Description = "Premium transportation in Alexandria",
                        LogoUrl = "alex_rides_logo.png",
                        Status = "Approved",
                        RejectionReason = string.Empty,
                        CreatedDate = DateTime.UtcNow.AddMonths(-5),
                        ApprovedDate = DateTime.UtcNow.AddMonths(-5).AddDays(2),
                        AverageRating = 0,
                        TotalRatings = 0,
                        SuperAdminName = "Nour Ahmed",
                        SuperAdminEmail = "nour@alexrides.com",
                        SuperAdminPhone = "+20123456790"
                    },
                    new Company
                    {
                        Name = "Delta Transport",
                        Email = "info@deltatransport.com",
                        Phone = "+20456789123",
                        Address = "22 El-Nasr Street, Mansoura",
                        Description = "Serving the Delta region with quality transportation",
                        LogoUrl = "delta_transport_logo.png",
                        Status = "Deleted",
                        RejectionReason = string.Empty,
                        CreatedDate = DateTime.UtcNow.AddMonths(-4),
                        ApprovedDate = DateTime.UtcNow.AddMonths(-4).AddDays(5),
                        AverageRating = 0,
                        TotalRatings = 0,
                        SuperAdminName = "Amr Mahmoud",
                        SuperAdminEmail = "amr@deltatransport.com",
                        SuperAdminPhone = "+20123456791"
                    },
                    new Company
                    {
                        Name = "Luxor Travels",
                        Email = "support@luxortravels.com",
                        Phone = "+20567891234",
                        Address = "5 Karnak Temple Road, Luxor",
                        Description = "Tourist transportation in Upper Egypt",
                        LogoUrl = "luxor_travels_logo.png",
                        Status = "Approved",
                        RejectionReason = string.Empty,
                        CreatedDate = DateTime.UtcNow.AddMonths(-3),
                        ApprovedDate = DateTime.UtcNow.AddMonths(-3).AddDays(4),
                        AverageRating = 0,
                        TotalRatings = 0,
                        SuperAdminName = "Heba Ali",
                        SuperAdminEmail = "heba@luxortravels.com",
                        SuperAdminPhone = "+20123456792"
                    },
                    new Company
                    {
                        Name = "Red Sea Mobility",
                        Email = "info@redseamobility.com",
                        Phone = "+20678912345",
                        Address = "30 El-Salam Road, Hurghada",
                        Description = "Transportation solutions in Red Sea resorts",
                        LogoUrl = "red_sea_mobility_logo.png",
                        Status = "Deleted",
                        RejectionReason = string.Empty,
                        CreatedDate = DateTime.UtcNow.AddMonths(-2),
                        ApprovedDate = DateTime.UtcNow.AddMonths(-2).AddDays(3),
                        AverageRating = 0,
                        TotalRatings = 0,
                        SuperAdminName = "Salem Mostafa",
                        SuperAdminEmail = "salem@redseamobility.com",
                        SuperAdminPhone = "+20123456793"
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

        private async Task SeedCitiesAsync()
        {
            _logger.LogInformation("Seeding cities...");
            
            // Check if cities already exist in the database
            if (!await _context.Cities.AnyAsync())
            {
                try
                {
                    // Define the path to the Cities.json file
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure", "Data", "DataSeed", "Cities.json");
                    
                    // If the file doesn't exist at the expected path, try looking in the current directory
                    if (!File.Exists(filePath))
                    {
                        // Try alternative paths
                        string[] possiblePaths = new[]
                        {
                            "Cities.json",
                            Path.Combine("Data", "DataSeed", "Cities.json"),
                            Path.Combine("Infrastructure", "Data", "DataSeed", "Cities.json"),
                            Path.Combine("..", "Infrastructure", "Data", "DataSeed", "Cities.json")
                        };
                        
                        foreach (var path in possiblePaths)
                        {
                            if (File.Exists(path))
                            {
                                filePath = path;
                                break;
                            }
                        }
                    }
                    
                    _logger.LogInformation("Reading cities data from file: {FilePath}", filePath);
                    
                    // Read the JSON file
                    string jsonData = await File.ReadAllTextAsync(filePath);
                    
                    // Deserialize JSON to list of city objects
                    var cityDtos = JsonSerializer.Deserialize<List<CitySeedDto>>(jsonData, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (cityDtos != null && cityDtos.Any())
                    {
                        _logger.LogInformation("Found {Count} cities in JSON file", cityDtos.Count);
                        
                        // Map to domain entities and add to context
                        var cities = cityDtos.Select(dto => new Domain.Entities.City
                        {
                            Name = dto.Name,
                            Governorate = dto.Governorate,
                            IsDeleted = false
                        }).ToList();
                        
                        await _context.Cities.AddRangeAsync(cities);
                        await _context.SaveChangesAsync();
                        
                        _logger.LogInformation("Successfully added {Count} cities to the database", cities.Count);
                    }
                    else
                    {
                        _logger.LogWarning("No cities found in the JSON file or file could not be properly deserialized");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error seeding cities from JSON file");
                    throw;
                }
            }
            else
            {
                _logger.LogInformation("Cities already exist in database. Skipping city seeding.");
            }
        }

        private async Task SeedStationsAsync()
        {
            _logger.LogInformation("Seeding stations...");
            
            // Check if stations already exist in the database
            if (!await _context.Stations.AnyAsync())
            {
                try
                {
                    // Define the path to the Stations.json file
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure", "Data", "DataSeed", "Stations.json");
                    
                    // If the file doesn't exist at the expected path, try looking in alternative locations
                    if (!File.Exists(filePath))
                    {
                        // Try alternative paths
                        string[] possiblePaths = new[]
                        {
                            "Stations.json",
                            Path.Combine("Data", "DataSeed", "Stations.json"),
                            Path.Combine("Infrastructure", "Data", "DataSeed", "Stations.json"),
                            Path.Combine("..", "Infrastructure", "Data", "DataSeed", "Stations.json")
                        };
                        
                        foreach (var path in possiblePaths)
                        {
                            if (File.Exists(path))
                            {
                                filePath = path;
                                break;
                            }
                        }
                    }
                    
                    _logger.LogInformation("Reading stations data from file: {FilePath}", filePath);
                    
                    // Read the JSON file
                    string jsonData = await File.ReadAllTextAsync(filePath);
                    
                    // Deserialize JSON to list of station objects
                    var stationDtos = JsonSerializer.Deserialize<List<StationSeedDto>>(jsonData, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (stationDtos != null && stationDtos.Any())
                    {
                        _logger.LogInformation("Found {Count} stations in JSON file", stationDtos.Count);
                        
                        // Get list of cities to validate city IDs
                        var cities = await _context.Cities.Select(c => c.Id).ToListAsync();
                        var companies = await _context.Companies.Select(c => c.Id).ToListAsync();
                        
                        // Map to domain entities and add to context
                        var stations = new List<Domain.Entities.Station>();
                        
                        foreach (var dto in stationDtos)
                        {
                            // Only add stations with valid CityId
                            if (cities.Contains(dto.CityId))
                            {
                                var station = new Domain.Entities.Station
                                {
                                    Name = dto.Name,
                                    Latitude = dto.Latitude,
                                    Longitude = dto.Longitude,
                                    CityId = dto.CityId,
                                    IsDeleted = false
                                };
                                
                                // Associate with company if provided and valid
                                if (dto.CompanyId.HasValue && companies.Contains(dto.CompanyId.Value))
                                {
                                    station.CompanyId = dto.CompanyId;
                                }
                                
                                stations.Add(station);
                            }
                            else
                            {
                                _logger.LogWarning("Skipping station {StationName} due to invalid CityId: {CityId}", 
                                    dto.Name, dto.CityId);
                            }
                        }
                        
                        await _context.Stations.AddRangeAsync(stations);
                        await _context.SaveChangesAsync();
                        
                        _logger.LogInformation("Successfully added {Count} stations to the database", stations.Count);
                    }
                    else
                    {
                        _logger.LogWarning("No stations found in the JSON file or file could not be properly deserialized");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error seeding stations from JSON file");
                    throw;
                }
            }
            else
            {
                _logger.LogInformation("Stations already exist in database. Skipping station seeding.");
            }
        }

        private class CitySeedDto
        {
            public int CityId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Governorate { get; set; } = string.Empty;
        }
        
        private class StationSeedDto
        {
            public int StationId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public int CityId { get; set; }
            public int? CompanyId { get; set; }
        }    
        //new dataseed
        //dataseed methods
        private async Task SeedRouteStationsAsync()
{
    _logger.LogInformation("Seeding route stations...");
    
    if (!await _context.RouteStations.AnyAsync())
    {
        var routes = await _context.Routes
            .Include(r => r.StartCity)
                .ThenInclude(c => c.Stations)
            .Include(r => r.EndCity)
                .ThenInclude(c => c.Stations)
            .ToListAsync();

        var routeStations = new List<RouteStation>();
        
        foreach (var route in routes)
        {
            // Get stations from start city
            var startCityStations = route.StartCity.Stations
                .OrderBy(x => Guid.NewGuid()) // Random order
                .Take(2) // Take 2 stations from start city
                .ToList();

            // Get stations from end city
            var endCityStations = route.EndCity.Stations
                .OrderBy(x => Guid.NewGuid()) // Random order
                .Take(2) // Take 2 stations from end city
                .ToList();

            var sequenceNumber = 1;

            // Add start city stations
            foreach (var station in startCityStations)
            {
                routeStations.Add(new RouteStation
                {
                    RouteId = route.Id,
                    StationId = station.Id,
                    SequenceNumber = sequenceNumber++,
                    IsActive = true
                });
            }

            // Add end city stations
            foreach (var station in endCityStations)
            {
                routeStations.Add(new RouteStation
                {
                    RouteId = route.Id,
                    StationId = station.Id,
                    SequenceNumber = sequenceNumber++,
                    IsActive = true
                });
            }
        }

        await _context.RouteStations.AddRangeAsync(routeStations);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Added {Count} route stations", routeStations.Count);
    }
}

private async Task SeedTripStationsAsync()
{
    _logger.LogInformation("Seeding trip stations...");
    
    if (!await _context.TripStations.AnyAsync())
    {
        var trips = await _context.Trips
            .Include(t => t.Route)
                .ThenInclude(r => r.RouteStations)
                    .ThenInclude(rs => rs.Station)
            .ToListAsync();

        var tripStations = new List<TripStation>();
        
        foreach (var trip in trips)
        {
            var routeStations = trip.Route.RouteStations
                .OrderBy(rs => rs.SequenceNumber)
                .ToList();

            for (int i = 0; i < routeStations.Count; i++)
            {
                var routeStation = routeStations[i];
                var baseTime = trip.DepartureTime;
                var minutesPerStation = trip.Route.EstimatedDuration / (routeStations.Count - 1);

                tripStations.Add(new TripStation
                {
                    TripId = trip.Id,
                    StationId = routeStation.StationId,
                    SequenceNumber = routeStation.SequenceNumber,
                    // First station has no arrival time
                    ArrivalTime = i == 0 ? null : baseTime.AddMinutes(minutesPerStation * (i)),
                    // Last station has arrival time as departure time
                    DepartureTime = i == routeStations.Count - 1 
                        ? baseTime.AddMinutes(minutesPerStation * i)
                        : baseTime.AddMinutes(minutesPerStation * i + 5), // 5 minutes stop at each station
                    IsActive = true
                });
            }
        }

        await _context.TripStations.AddRangeAsync(tripStations);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Added {Count} trip stations", tripStations.Count);
    }
}

        private async Task SeedBusesAsync()
{
    _logger.LogInformation("Seeding buses...");
    
    if (!await _context.Buses.AnyAsync())
    {
        var companies = await _context.Companies.ToListAsync();
        
        var buses = new List<Bus>();
        foreach (var company in companies)
        {
            // Add 3 buses per company
            for (int i = 1; i <= 3; i++)
            {
                buses.Add(new Bus
                {
                    RegistrationNumber = $"{company.Name.Substring(0, 3).ToUpper()}-{i:D3}",
                    Capacity = 45 + (i % 3) * 5, // Varies between 45, 50, 55
                    Model = $"Coach {2020 + i}",
                    YearOfManufacture = 2020 + i,
                    IsActive = true,
                    CompanyId = company.Id,
                    AmenityDescription = "WiFi, AC, USB Charging, Comfortable Seats"
                });
            }
        }

        await _context.Buses.AddRangeAsync(buses);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Added {Count} buses", buses.Count);
    }
}

private async Task SeedRoutesAsync()
{
    _logger.LogInformation("Seeding routes...");
    
    if (!await _context.Routes.AnyAsync())
    {
        var companies = await _context.Companies.ToListAsync();
        var cities = await _context.Cities.ToListAsync();
        
        var routes = new List<Route>();
        foreach (var company in companies)
        {
            // Create routes between major cities for each company
            for (int i = 0; i < cities.Count - 1; i++)
            {
                routes.Add(new Route
                {
                    Name = $"{cities[i].Name} - {cities[i + 1].Name}",
                    StartCityId = cities[i].Id,
                    EndCityId = cities[i + 1].Id,
                    Distance = 100 + (i * 50), // Random distance
                    EstimatedDuration = 120 + (i * 30), // Duration in minutes
                    CompanyId = company.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                });
            }
        }

        await _context.Routes.AddRangeAsync(routes);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Added {Count} routes", routes.Count);
    }
}

private async Task SeedTripsAsync()
{
    _logger.LogInformation("Seeding trips...");
    
    if (!await _context.Trips.AnyAsync())
    {
        var routes = await _context.Routes
            .Include(r => r.Company)
            .ToListAsync();
            
        var drivers = await _context.Drivers.ToListAsync();
        var buses = await _context.Buses.ToListAsync();
        
        var trips = new List<Trip>();
        var random = new Random();
        
        foreach (var route in routes)
        {
            // Create 3 trips per route
            for (int i = 0; i < 3; i++)
            {
                var departureTime = DateTime.UtcNow.AddDays(i + 1).Date
                    .AddHours(8 + (i * 4)); // Trips at 8AM, 12PM, 4PM
                
                var driver = drivers
                    .FirstOrDefault(d => d.CompanyId == route.CompanyId);
                    
                var bus = buses
                    .FirstOrDefault(b => b.CompanyId == route.CompanyId && b.IsActive);
                
                if (driver != null && bus != null)
                {
                    trips.Add(new Trip
                    {
                        RouteId = route.Id,
                        DepartureTime = departureTime,
                        ArrivalTime = departureTime.AddMinutes(route.EstimatedDuration),
                        DriverId = driver.Id,
                        BusId = bus.Id,
                        CompanyId = route.CompanyId,
                        IsCompleted = false,
                        AvailableSeats = bus.Capacity,
                        Price = 100 + (route.Distance * 0.5M) // Base price + distance factor
                    });
                }
            }
        }

        await _context.Trips.AddRangeAsync(trips);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Added {Count} trips", trips.Count);
    }
}

// private async Task SeedBookingsAndTicketsAsync()
// {
//     _logger.LogInformation("Seeding bookings and tickets...");
    
//     if (!await _context.Bookings.AnyAsync())
//     {
//         var passengers = await _context.Passengers.ToListAsync();
//         var trips = await _context.Trips
//             .Include(t => t.Bus)
//             .ToListAsync();
            
//         var bookings = new List<Booking>();
//         var tickets = new List<Ticket>();
//         var random = new Random();
        
//         foreach (var passenger in passengers)
//         {
//             // Create 2 bookings per passenger
//             for (int i = 0; i < 2; i++)
//             {
//                 var trip = trips[random.Next(trips.Count)];
//                 var numberOfTickets = random.Next(1, 4); // 1-3 tickets per booking
                
//                 var booking = new Booking
//                 {
//                     PassengerId = passenger.Id,
//                     TripId = trip.Id,
//                     BookingDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
//                     Status = BookingStatus.Confirmed,
//                     TotalPrice = trip.Price * numberOfTickets
//                 };
                
//                 bookings.Add(booking);
                
//                 // Create tickets for the booking
//                 for (int j = 0; j < numberOfTickets; j++)
//                 {
//                     tickets.Add(new Ticket
//                     {
//                         TripId = trip.Id,
//                         PassengerId = passenger.Id,
//                         BookingId = booking.Id,
//                         SeatNumber = j + 1,
//                         Price = trip.Price,
//                         PurchaseDate = booking.BookingDate,
//                         IsUsed = false
//                     });
//                 }
//             }
//         }

//         await _context.Bookings.AddRangeAsync(bookings);
//         await _context.Tickets.AddRangeAsync(tickets);
//         await _context.SaveChangesAsync();
        
//         _logger.LogInformation("Added {BookingCount} bookings and {TicketCount} tickets", 
//             bookings.Count, tickets.Count);
//     }
// }

        // Add these DTOs at the bottom of the Ra7alaDataSeed class
private class BusSeedDto
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Model { get; set; } = string.Empty;
    public int YearOfManufacture { get; set; }
    public bool IsActive { get; set; }
    public int CompanyId { get; set; }
    public string AmenityDescription { get; set; } = string.Empty;
}

private class RouteSeedDto
{
    public string Name { get; set; } = string.Empty;
    public int StartCityId { get; set; }
    public int EndCityId { get; set; }
    public int Distance { get; set; }
    public int EstimatedDuration { get; set; }
    public int CompanyId { get; set; }
}

private class TripSeedDto
{
    public int RouteId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string DriverId { get; set; } = string.Empty;
    public int BusId { get; set; }
    public int CompanyId { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
}    
    }
}