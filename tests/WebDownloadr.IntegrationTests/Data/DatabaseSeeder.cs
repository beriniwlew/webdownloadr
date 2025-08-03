using Microsoft.Extensions.Logging.Abstractions;
using WebDownloadr.Infrastructure.Data;

namespace WebDownloadr.IntegrationTests.Data;

public class DatabaseSeederTests : BaseEfRepoTestFixture
{
  [Fact]
  public async Task SeedsContributors()
  {
    var seeder = new DatabaseSeeder(_dbContext, NullLogger<DatabaseSeeder>.Instance);

    await seeder.SeedAsync();

    var contributor = await _dbContext.Contributors
      .FirstOrDefaultAsync(c => c.Name == SeedData.Contributor1.Name);

    contributor.ShouldNotBeNull();
  }
}

