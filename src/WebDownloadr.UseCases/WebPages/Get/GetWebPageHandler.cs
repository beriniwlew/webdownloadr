using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Get;

public class GetWebPageHandler(IReadRepository<WebPage> repository)
  : IQueryHandler<GetWebPageQuery, Result<WebPageDTO>>
{
  public async Task<Result<WebPageDTO>> Handle(GetWebPageQuery request, CancellationToken cancellationToken)
  {
    var entity = await repository.GetByIdAsync(WebPageId.From(request.WebPageId), cancellationToken);
    if (entity == null) return Result.NotFound();

    return new WebPageDTO(entity.Id.Value, entity.Url.Value, entity.Status.ToString());
  }
}
