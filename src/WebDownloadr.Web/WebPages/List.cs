using WebDownloadr.UseCases.WebPages;
using WebDownloadr.UseCases.WebPages.List;
using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.Web.WebPages;

public class List(IMediator _mediator) : EndpointWithoutRequest<WebPageListResponse>
{
  public override void Configure()
  {
    Get("/WebPages");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ListWebPagesQuery(null, null), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new WebPageListResponse
      {
        WebPages = result.Value.Select(p => new WebPageRecord(p.Id, p.Url, DownloadStatus.FromName(p.Status))).ToList()
      };
    }
  }
}
