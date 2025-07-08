using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.IntegrationTests.Data.WebPages;

public class EfRepositoryWebPageUpdate : BaseEfRepoWebPageFixture
{
  [Fact]
  public async Task UpdatesWebPageAfterAdding()
  {
    var repository = GetRepository();
    var page = new WebPage(WebPageUrl.From("https://initial.com"));
    await repository.AddAsync(page);
    _dbContext.Entry(page).State = EntityState.Detached;

    var fetched = (await repository.ListAsync()).First(p => p.Url.Value == "https://initial.com");
    fetched.UpdateStatus(DownloadStatus.DownloadCompleted);

    await repository.UpdateAsync(fetched);

    var updated = (await repository.ListAsync()).First(p => p.Id == fetched.Id);
    updated.Status.ShouldBe(DownloadStatus.DownloadCompleted);
  }
}
