using WebDownloadr.Infrastructure.Data;
using WebDownloadr.Web.Contributors;

namespace WebDownloadr.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class ContributorDelete(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task DeletesContributorGivenValidId()
  {
    string route = DeleteContributorRequest.BuildRoute(SeedData.Contributor1.Id);
    _ = await _client.DeleteAndEnsureNoContentAsync(route);
  }

  [Fact]
  public async Task ReturnsNotFoundGivenInvalidId()
  {
    string route = DeleteContributorRequest.BuildRoute(1000);
    _ = await _client.DeleteAndEnsureNotFoundAsync(route);
  }
}
