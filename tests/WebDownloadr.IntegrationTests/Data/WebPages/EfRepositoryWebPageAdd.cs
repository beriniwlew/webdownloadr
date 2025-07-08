using System;
using System.Linq;
using Shouldly;
using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.IntegrationTests.Data.WebPages;

public class EfRepositoryWebPageAdd : BaseEfRepoWebPageFixture
{
  [Fact]
  public async Task AddsWebPageAndSetsId()
  {
    var repository = GetRepository();
    var page = new WebPage(WebPageUrl.From("https://example.com"));

    await repository.AddAsync(page);

    var result = (await repository.ListAsync()).FirstOrDefault();
    result.ShouldNotBeNull();
    result.Url.ShouldBe(page.Url);
    result.Status.ShouldBe(page.Status);
    result.Id.Value.ShouldNotBe(Guid.Empty);
  }
}
