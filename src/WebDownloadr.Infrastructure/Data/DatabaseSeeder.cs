namespace WebDownloadr.Infrastructure.Data;

public sealed class DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger) : IDatabaseSeeder
{
  public async Task SeedAsync()
  {
    try
    {
      context.Database.EnsureCreated();
      await SeedData.InitializeAsync(context);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
  }
}

