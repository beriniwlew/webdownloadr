using Microsoft.Data.Sqlite;
using WebDownloadr.Core.ContributorAggregate;
using WebDownloadr.Infrastructure.Data;
using WebDownloadr.Infrastructure.Data.Queries;
using WebDownloadr.UseCases.Contributors;

namespace WebDownloadr.IntegrationTests.Data.Contributors;

public class ListContributorsQueryServiceTests
{
  [Fact]
  public async Task ReturnsAllContributors()
  {
    using var connection = new SqliteConnection("DataSource=:memory:");
    connection.Open();

    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseSqlite(connection)
      .Options;

    var dispatcher = Substitute.For<IDomainEventDispatcher>();
    using var context = new AppDbContext(options, dispatcher);
    context.Database.EnsureCreated();

    context.Contributors.AddRange([
      new Contributor("One"),
      new Contributor("Two")
    ]);
    await context.SaveChangesAsync();

    var service = new ListContributorsQueryService(context);

    var result = (await service.ListAsync()).ToList();

    result.Count.ShouldBe(2);
    result.Any(c => c.Name == "One").ShouldBeTrue();
    result.Any(c => c.Name == "Two").ShouldBeTrue();
  }
}
