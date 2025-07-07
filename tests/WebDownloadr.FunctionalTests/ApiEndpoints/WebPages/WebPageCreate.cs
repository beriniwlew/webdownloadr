using System.Text;
using System.Text.Json;
using WebDownloadr.Web.WebPages;

namespace WebDownloadr.FunctionalTests.ApiEndpoints.WebPages;

[Collection("Sequential")]
public class WebPageCreate(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task ReturnsNewGuid()
  {
    var request = new CreateWebPageRequest { Url = "https://dot.net" };
    var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

    var result = await _client.PostAndDeserializeAsync<CreateWebPageResponse>(CreateWebPageRequest.Route, content);

    result.Id.ShouldNotBe(Guid.Empty);
    result.Url.ShouldBe(request.Url);
  }
}
