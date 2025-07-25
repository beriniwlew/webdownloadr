namespace WebDownloadr.UseCases.WebPages.List;

/// <summary>
/// Handles <see cref="ListWebPagesQuery"/> by invoking the query service.
/// </summary>
public class ListWebPagesHandler(IListWebPagesQueryService query)
  : IQueryHandler<ListWebPagesQuery, Result<IEnumerable<WebPageDTO>>>
{
  /// <summary>
  /// Lists all <see cref="WebPage"/> records.
  /// </summary>
  /// <param name="request">Query instance (unused).</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  /// <returns>Collection of pages wrapped in a successful result.</returns>
  public async Task<Result<IEnumerable<WebPageDTO>>> Handle(ListWebPagesQuery request, CancellationToken cancellationToken)
  {
    var result = await query.ListAsync(cancellationToken);
    return Result.Success(result);
  }
}
