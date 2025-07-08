using Ardalis.HttpClientTestExtensions;
using Shouldly;
using WebDownloadr.Web.WebPages;

namespace WebDownloadr.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class WebPageCreateList(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CanCreateWebPage()
    {
        var request = new CreateWebPageRequest { Url = "https://example.com" };
        var create = await _client.PostAndDeserializeAsync<CreateWebPageResponse>(CreateWebPageRequest.Route, StringContentHelpers.FromModelAsJson(request));

        create.Url.ShouldBe("https://example.com");

        // verify create succeeded by returning same url
    }
}
