namespace WebDownloadr.UseCases.WebPages.List;

/// <summary>
/// Handles <see cref="ListWebPagesQuery"/> by invoking the query service.
/// </summary>
public class ListWebPagesHandler(IListWebPagesQueryService query)
  : IQueryHandler<ListWebPagesQuery, Result<IEnumerable<WebPageDTO>>>
{
  public async Task<Result<IEnumerable<WebPageDTO>>> Handle(ListWebPagesQuery request, CancellationToken cancellationToken)
  {
    var result = await query.ListAsync();
    return Result.Success(result);
  }
}
