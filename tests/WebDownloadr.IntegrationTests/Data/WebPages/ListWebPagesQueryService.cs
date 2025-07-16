using Microsoft.Data.Sqlite;
using WebDownloadr.Core.WebPageAggregate;
using WebDownloadr.Infrastructure.Data;
using WebDownloadr.Infrastructure.Data.Queries;
using WebDownloadr.UseCases.WebPages;

namespace WebDownloadr.IntegrationTests.Data.WebPages;

public class ListWebPagesQueryServiceTests
{
  [Fact]
  public async Task ReturnsAllWebPages()
  {
    using var connection = new SqliteConnection("DataSource=:memory:");
    connection.Open();

    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseSqlite(connection)
      .Options;

    var dispatcher = Substitute.For<IDomainEventDispatcher>();
    using var context = new AppDbContext(options, dispatcher);
    context.Database.EnsureCreated();

    context.WebPages.AddRange([
      new WebPage(WebPageUrl.From("https://one.com")),
      new WebPage(WebPageUrl.From("https://two.com"))
    ]);
    await context.SaveChangesAsync();

    var service = new ListWebPagesQueryService(context);

    var result = (await service.ListAsync()).ToList();

    result.Count.ShouldBe(2);
    result.Any(p => p.Url == "https://one.com").ShouldBeTrue();
    result.Any(p => p.Url == "https://two.com").ShouldBeTrue();
  }
}
