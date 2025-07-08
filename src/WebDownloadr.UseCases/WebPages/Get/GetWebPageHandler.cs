using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Get;

/// <summary>
/// Handles <see cref="GetWebPageQuery"/> by retrieving the page entity from the repository.
/// </summary>
  public class GetWebPageHandler(IReadRepository<WebPage> repository)
    : IQueryHandler<GetWebPageQuery, Result<WebPageDTO>>
  {
  /// <summary>
  /// Retrieves a <see cref="WebPage"/> by identifier.
  /// </summary>
  /// <param name="request">Query containing the identifier.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  /// <returns>Result with the page details or <see cref="Result.NotFound"/>.</returns>
  public async Task<Result<WebPageDTO>> Handle(GetWebPageQuery request, CancellationToken cancellationToken)
  {
    var entity = await repository.GetByIdAsync(WebPageId.From(request.WebPageId), cancellationToken);
    if (entity == null) return Result.NotFound();

    return new WebPageDTO(entity.Id.Value, entity.Url.Value, entity.Status.Name);
  }
}
