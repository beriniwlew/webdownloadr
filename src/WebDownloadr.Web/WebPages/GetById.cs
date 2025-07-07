using WebDownloadr.UseCases.WebPages.Get;
using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.Web.WebPages;

public class GetById(IMediator _mediator) : Endpoint<GetWebPageByIdRequest, WebPageRecord>
{
  public override void Configure()
  {
    Get(GetWebPageByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetWebPageByIdRequest request, CancellationToken cancellationToken)
  {
    var query = new GetWebPageQuery(request.WebPageId);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      var dto = result.Value;
      Response = new WebPageRecord(dto.Id, dto.Url, DownloadStatus.FromName(dto.Status));
    }
  }
}
