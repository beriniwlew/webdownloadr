using WebDownloadr.UseCases.WebPages.Download.RetryDownload;

namespace WebDownloadr.Web.WebPages;

public class RetryDownload(IMediator _mediator) : Endpoint<RetryDownloadRequest, Guid>
{
  public override void Configure()
  {
    Post(RetryDownloadRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(RetryDownloadRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new RetryDownloadCommand(request.WebPageId), cancellationToken);

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
