using WebDownloadr.Infrastructure.Data;
using WebDownloadr.Web.WebPages;

namespace WebDownloadr.FunctionalTests.ApiEndpoints.WebPages;

[Collection("Sequential")]
public class WebPageList(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task ReturnsSeedWebPages()
  {
    var result = await _client.GetAndDeserializeAsync<WebPageListResponse>("/WebPages");

    result.WebPages.Count.ShouldBe(2);
    result.WebPages.ShouldContain(p => p.Url == SeedData.WebPage1.Url.Value);
    result.WebPages.ShouldContain(p => p.Url == SeedData.WebPage2.Url.Value);
  }
}
