using Microsoft.EntityFrameworkCore;
using WebDownloadr.Infrastructure.Data;
using WebDownloadr.UseCases.WebPages;
using WebDownloadr.UseCases.WebPages.List;

namespace WebDownloadr.Infrastructure.Data.Queries;

public class ListWebPagesQueryService(AppDbContext db) : IListWebPagesQueryService
{
  public async Task<IEnumerable<WebPageDTO>> ListAsync()
  {
    return await db.WebPages
      .AsNoTracking()
      .Select(p => new WebPageDTO(p.Id.Value, p.Url.Value, p.Status.Name))
      .ToListAsync();
  }
}
