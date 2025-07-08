using System.Linq;
using Shouldly;
using WebDownloadr.Core.WebPageAggregate;
namespace WebDownloadr.IntegrationTests.Data.WebPages;

public class EfRepositoryWebPageDelete : BaseEfRepoWebPageFixture
{
  [Fact]
  public async Task DeletesWebPageAfterAdding()
  {
    var repository = GetRepository();
    var page = new WebPage(WebPageUrl.From("https://todelete.com"));
    await repository.AddAsync(page);

    await repository.DeleteAsync(page);

    (await repository.ListAsync()).ShouldNotContain(p => p.Url.Value == "https://todelete.com");
  }
}
