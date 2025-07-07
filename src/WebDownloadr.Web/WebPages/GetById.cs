using WebDownloadr.UseCases.WebPages.Get;

namespace WebDownloadr.Web.WebPages;

public class GetById(IMediator _mediator)
  : Endpoint<GetWebPageByIdRequest, WebPageRecord>
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
      Response = new WebPageRecord(result.Value.Id, result.Value.Url, result.Value.Status);
    }
  }
}
