using WebDownloadr.UseCases.WebPages;
using WebDownloadr.UseCases.WebPages.List;
using WebDownloadr.Infrastructure.Data;

namespace WebDownloadr.Infrastructure.Data.Queries;

public class ListWebPagesQueryService(AppDbContext db) : IListWebPagesQueryService
{
  public async Task<IEnumerable<WebPageDTO>> ListAsync()
  {
    var result = await db.Database.SqlQuery<WebPageDTO>(
      $"SELECT Id, Url, Status FROM WebPages")
      .ToListAsync();

    return result;
  }
}
