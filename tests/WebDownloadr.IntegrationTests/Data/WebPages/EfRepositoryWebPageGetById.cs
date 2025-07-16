using System.Linq;
using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.IntegrationTests.Data.WebPages;

public class EfRepositoryWebPageGetById : BaseEfRepoWebPageFixture
{
  [Fact]
  public async Task GetsWebPageByIdAfterAdding()
  {
    var repository = GetRepository();
    var page = new WebPage(WebPageUrl.From("https://getbyid.com"));

    await repository.AddAsync(page);

    var fetched = await repository.GetByIdAsync(page.Id);

    fetched.ShouldNotBeNull();
    fetched!.Url.ShouldBe(page.Url);
    fetched.Status.ShouldBe(page.Status);
  }
}
