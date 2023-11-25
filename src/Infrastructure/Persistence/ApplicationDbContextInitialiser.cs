using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            if (!_context.Database.EnsureCreated())
            {
                await TrySeedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var adminRole = new IdentityRole("Admin");
        if (_roleManager.Roles.All(r => r.Name != adminRole.Name))
        {
            await _roleManager.CreateAsync(adminRole);
        }

        var garageRole = new IdentityRole("Garage");
        if (_roleManager.Roles.All(r => r.Name != garageRole.Name))
        {
            await _roleManager.CreateAsync(garageRole);
        }

        var userRole = new IdentityRole("User");
        if (_roleManager.Roles.All(r => r.Name != userRole.Name))
        {
            await _roleManager.CreateAsync(userRole);
        }

        await _context.SaveChangesAsync();

        //// Default users
        //var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        //if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        //{
        //    await _userManager.CreateAsync(administrator, "Administrator1!");
        //    if (!string.IsNullOrWhiteSpace(administratorRole.Name))
        //    {
        //        await _userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
        //    }
        //}

        //// Default data
        //// Seed, if necessary
        //if (!_context.TodoLists.Any())
        //{
        //    _context.TodoLists.Add(new TodoList
        //    {
        //        Title = "Todo List",
        //        Items =
        //        {
        //            new TodoItem { Title = "Make a todo list 📃" },
        //            new TodoItem { Title = "Check off the first item ✅" },
        //            new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
        //            new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
        //        }
        //    });

        //    await _context.SaveChangesAsync();
        //}

    }

    private static VehicleLookupItem GetTestVehicleLookup()
    {
        return new VehicleLookupItem
        {
            //Id = Guid.NewGuid(),
            //VehicleType = VehicleLookupType.Car,
            //Make = "Honda",
            //Model = "Civic",
            //Year = 2010,
            //Engine = "1.8L",
            //Transmission = "Automatic",
            //FuelType = "Petrol",
            //BodyType = "Sedan",
            //DriveType = "FWD",
            //Colour = "Black",
            //Vin = "12345678901234567",
            //Registration = "ABC123",
            //RegistrationExpiry = DateTime.UtcNow.AddYears(1),
            //Odometer = 100000,
            //OdometerUnit = "km",
            //ImageUrl = "https://www.honda.com.au/content/dam/honda/cars/models/civic-sedan/overview/hero/hero-civic-sedan-1.5l-vti-lx-pearl-white-pearl.jpg"
        };
    }

    private static VehicleServiceLogItem[] GetTestVehicleServiceLogs()
    {
        return new[]
        {
            new VehicleServiceLogItem
            {
            }
        };
    }

    private static VehicleTimelineItem[] GetTestVehicleTimeline()
    {
        return new[]
        {
            new VehicleTimelineItem
            {
            }
        };
    }

    /// <summary>
    /// We can start conversations with this garage.
    /// </summary>
    private static GarageLookupItem GetTestGarageLookup()
    {
        return new GarageLookupItem
        {

        };
    }



}
