using WebDownloadr.Core.ContributorAggregate;
using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.Infrastructure.Data;

public static class SeedData
{
  public static readonly Contributor Contributor1 = new("Ardalis");
  public static readonly Contributor Contributor2 = new("Snowfrog");
  public static readonly WebPage WebPage1 = new(WebPageUrl.From("https://example.com"));
  public static readonly WebPage WebPage2 = new(WebPageUrl.From("https://github.com"));

  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (await dbContext.Contributors.AnyAsync()) return; // DB has been seeded

    await PopulateTestDataAsync(dbContext);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);
    dbContext.WebPages.AddRange([WebPage1, WebPage2]);
    await dbContext.SaveChangesAsync();
  }
}
