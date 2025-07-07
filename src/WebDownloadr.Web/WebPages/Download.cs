using WebDownloadr.UseCases.WebPages.Download.DownloadWebPage;

namespace WebDownloadr.Web.WebPages;

public class Download(IMediator _mediator) : Endpoint<DownloadWebPageRequest, Guid>
{
  public override void Configure()
  {
    Post(DownloadWebPageRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DownloadWebPageRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DownloadWebPageCommand(request.WebPageId), cancellationToken);

    if (result.IsSuccess)
    {
      Response = result.Value;
    }
    else if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
    }
  }
}
