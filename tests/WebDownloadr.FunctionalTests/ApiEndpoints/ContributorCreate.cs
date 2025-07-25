using Ardalis.HttpClientTestExtensions;
using WebDownloadr.Web.Contributors;

namespace WebDownloadr.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class ContributorCreate(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task CanCreateContributor()
  {
    var request = new CreateContributorRequest { Name = "Test Contributor" };
    var create = await _client.PostAndDeserializeAsync<CreateContributorResponse>(CreateContributorRequest.Route, StringContentHelpers.FromModelAsJson(request));

    create.Name.ShouldBe("Test Contributor");
    create.Id.ShouldBeGreaterThan(0);
  }

  [Fact]
  public async Task ReturnsBadRequestGivenInvalidName()
  {
    var request = new CreateContributorRequest { Name = string.Empty };
    await _client.PostAndEnsureBadRequestAsync(CreateContributorRequest.Route, StringContentHelpers.FromModelAsJson(request));
  }
}
