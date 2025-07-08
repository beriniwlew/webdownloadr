using WebDownloadr.UseCases.WebPages.Download.CancelDownload;

namespace WebDownloadr.Web.WebPages;

/// <summary>
/// API endpoint used to cancel a running download.
/// </summary>
public class CancelDownload(IMediator _mediator) : Endpoint<CancelDownloadRequest, Guid>
{
  /// <inheritdoc />
  public override void Configure()
  {
    Post(CancelDownloadRequest.Route);
    AllowAnonymous();
  }

  /// <inheritdoc />
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
