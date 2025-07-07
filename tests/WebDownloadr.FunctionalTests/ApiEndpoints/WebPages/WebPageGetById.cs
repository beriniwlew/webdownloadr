using WebDownloadr.Infrastructure.Data;
using WebDownloadr.Web.WebPages;

namespace WebDownloadr.FunctionalTests.ApiEndpoints.WebPages;

[Collection("Sequential")]
public class WebPageGetById(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task ReturnsSeedWebPageGivenValidId()
  {
    var route = GetWebPageByIdRequest.BuildRoute(SeedData.WebPage1.Id.Value);
    var result = await _client.GetAndDeserializeAsync<WebPageRecord>(route);

    result.Id.ShouldBe(SeedData.WebPage1.Id.Value);
    result.Url.ShouldBe(SeedData.WebPage1.Url.Value);
  }

  [Fact]
  public async Task ReturnsNotFoundGivenUnknownId()
  {
    string route = GetWebPageByIdRequest.BuildRoute(Guid.NewGuid());
    _ = await _client.GetAndEnsureNotFoundAsync(route);
  }
}
