using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.List;

public class ListWebPagesHandler(IReadRepository<WebPage> repository)
  : IQueryHandler<ListWebPagesQuery, Result<IEnumerable<WebPageDTO>>>
{
  public async Task<Result<IEnumerable<WebPageDTO>>> Handle(ListWebPagesQuery request, CancellationToken cancellationToken)
  {
    var entities = await repository.ListAsync(cancellationToken);
    var result = entities.Select(e => new WebPageDTO(e.Id.Value, e.Url.Value, e.Status.ToString()));
    return Result.Success(result);
  }
}
