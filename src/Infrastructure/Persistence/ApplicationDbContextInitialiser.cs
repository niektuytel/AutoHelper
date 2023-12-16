using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Hangfire.MediatR;
using AutoHelper.Infrastructure.Identity;
using MediatR;
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
    private readonly IMediator _mediator;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMediator mediator)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _mediator = mediator;
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

            // Default (development) garages
            var testGarageLookup = GetTestGarageLookup();
            if (_context.GarageLookups.FirstOrDefault(x => x.Name == testGarageLookup.Name) == null)
            {
                _context.GarageLookups.Add(testGarageLookup);
                await _context.SaveChangesAsync();
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

        // Default (development) users
        var admin = new ApplicationUser { Id = "admin@autohelper|e13a0844", UserName = "admin@autohelper.nl", Email = "admin@autohelper.nl" };
        if (_userManager.Users.All(u => u.UserName != admin.UserName))
        {
            var result = await _userManager.CreateAsync(admin, "Autohelper123!");
            if (result.Succeeded && !string.IsNullOrWhiteSpace(adminRole.Name))
            {
                await _userManager.AddToRolesAsync(admin, new[] { adminRole.Name });
            }
        }

        var garage = new ApplicationUser { Id = "garage@autohelper|5ea3782cf852", UserName = "garage@autohelper.nl", Email = "garage@autohelper.nl" };
        if (_userManager.Users.All(u => u.UserName != garage.UserName))
        {
            var result = await _userManager.CreateAsync(garage, "Autohelper123!");
            if (result.Succeeded && !string.IsNullOrWhiteSpace(garageRole.Name))
            {
                await _userManager.AddToRolesAsync(garage, new[] { garageRole.Name });
            }
        }

    }

    /// <summary>
    /// We can start conversations with this garage.
    /// </summary>
    private static GarageLookupItem GetTestGarageLookup()
    {
        var random = new Random();
        return new GarageLookupItem
        {
            Identifier = "Garage_1337",
            GarageId = null, // Assuming GarageId is not set initially
            Name = "Garage 1337",
            Address = "1234 Test Street",
            City = "TestCity",
            Location = null, // Assuming location is not known initially
            Image = null,
            ImageThumbnail = null,
            Status = random.Next(2) == 0 ? "Open" : "Closed",
            PhoneNumber = "555-" + random.Next(1000, 9999),
            WhatsappNumber = "+1555" + random.Next(1000, 9999),
            EmailAddress = $"contact@garage{random.Next(1, 101)}.com",
            ConversationContactEmail = random.Next(2) == 0 ? $"contact@garage{random.Next(1, 101)}.com" : null,
            ConversationContactWhatsappNumber = random.Next(2) == 0 ? "+1555" + random.Next(1000, 9999) : null,
            DaysOfWeekString = string.Join(",", Enumerable.Range(1, 7).OrderBy(_ => random.Next()).Take(random.Next(1, 8))),
            Website = "www.garage" + random.Next(1, 101) + ".com",
            Rating = (float)Math.Round(random.NextDouble() * 4 + 1, 1),
            UserRatingsTotal = random.Next(1, 501),
            Created = DateTime.Now,
            CreatedBy = "System",
            LastModified = DateTime.Now,
            LastModifiedBy = "System"
        };
    }

    public async Task StartSyncTasksWhenEmpty()
    {
        // Insert All when empty
        var vehicleCount = await _context.VehicleLookups.CountAsync();
        if (vehicleCount == 0)
        {
            var command = new UpsertVehicleLookupsCommand();
            var queue = $"{nameof(UpsertVehicleLookupsCommand)}";
            var title = $"{nameof(StartSyncTasksWhenEmpty)} lookups";
            _mediator.Enqueue(queue, title, command);
        }

    }


}
