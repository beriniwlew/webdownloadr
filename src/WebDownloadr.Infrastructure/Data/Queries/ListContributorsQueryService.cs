using Microsoft.EntityFrameworkCore;
using WebDownloadr.UseCases.Contributors;
using WebDownloadr.UseCases.Contributors.List;

namespace WebDownloadr.Infrastructure.Data.Queries;

public class ListContributorsQueryService(AppDbContext _db) : IListContributorsQueryService
{
  // You can use EF, Dapper, SqlClient, etc. for queries -
  // this is just an example

  public async Task<IEnumerable<ContributorDTO>> ListAsync(CancellationToken ct)
  {
    return await _db.Contributors
      .AsNoTracking()
      .Select(c => new ContributorDTO(c.Id, c.Name, (object?)c.PhoneNumber != null ? c.PhoneNumber.Number : null))
      .ToListAsync();
  }
}
