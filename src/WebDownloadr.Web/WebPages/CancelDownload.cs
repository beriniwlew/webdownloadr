using WebDownloadr.UseCases.WebPages.Download.CancelDownload;

namespace WebDownloadr.Web.WebPages;

public class CancelDownload(IMediator _mediator) : Endpoint<CancelDownloadRequest, Guid>
{
  public override void Configure()
  {
    Post(CancelDownloadRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancelDownloadRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CancelDownloadCommand(request.WebPageId), cancellationToken);

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
