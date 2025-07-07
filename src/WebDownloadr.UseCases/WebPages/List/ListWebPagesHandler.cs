namespace WebDownloadr.UseCases.WebPages.List;

public class ListWebPagesHandler(IListWebPagesQueryService query)
  : IQueryHandler<ListWebPagesQuery, Result<IEnumerable<WebPageDTO>>>
{
  public async Task<Result<IEnumerable<WebPageDTO>>> Handle(ListWebPagesQuery request, CancellationToken cancellationToken)
  {
    var result = await query.ListAsync();
    return Result.Success(result);
  }
}
