using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Hangfire.Persistence;

/// <summary>
/// Only used to create the database context for Hangfire automaticly when not exist.
/// Removed the manual creation of the database.
/// </summary>
public class HangfireDbContext : DbContext
{
    public HangfireDbContext(DbContextOptions<HangfireDbContext> options) : base(options)
    { }
}
