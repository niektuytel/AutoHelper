using AutoHelper.Domain.Entities.Garages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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
            // Development garage
            var testGarageLookup = GetTestGarageLookup();
            if (_context.GarageLookups.FirstOrDefault(x => x.Identifier == testGarageLookup.Identifier) == null)
            {
                _context.GarageLookups.Add(testGarageLookup);
                await _context.SaveChangesAsync();

                var testGarageLookupService = GetTestGarageLookupService();
                _context.GarageLookupServices.Add(testGarageLookupService);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private static GarageLookupItem GetTestGarageLookup()
    {
        var random = new Random();
        return new GarageLookupItem
        {
            Identifier = "Garage_1337",
            GarageId = null, // Assuming GarageId is not set initially
            Name = "Garage 1337",
            Address = "Heiweg 6",
            City = "Oud-Alblas",
            Location = null, // Assuming location is not known initially
            Image = null,
            ImageThumbnail = null,
            Status = random.Next(2) == 0 ? "Open" : "Closed",
            PhoneNumber = "0185642431",
            WhatsappNumber = "+31612395778",
            EmailAddress = $"contact@autohelper.nl",
            ConversationContactEmail = $"contact@autohelper.nl",
            DaysOfWeekString = string.Join(",", Enumerable.Range(1, 7).OrderBy(_ => random.Next()).Take(random.Next(1, 8))),
            Website = "https://www.autohelper.nl",
            Rating = (float)Math.Round(random.NextDouble() * 4 + 1, 1),
            UserRatingsTotal = random.Next(1, 501),
            Created = DateTime.Now,
            CreatedBy = "System",
            LastModified = DateTime.Now,
            LastModifiedBy = "System"
        };
    }

    private static GarageLookupServiceItem GetTestGarageLookupService()
    {
        return new GarageLookupServiceItem
        {
            GarageLookupIdentifier = "Garage_1337",
            Type = GarageServiceType.Service,
            VehicleType = Domain.Entities.Vehicles.VehicleType.Any,
            Title = "MOT Service",
            Description = "Change the oil in the engine",
            ExpectedNextDateIsRequired = true,
            ExpectedNextOdometerReadingIsRequired = true
        };
    }
}
