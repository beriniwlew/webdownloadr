using WebDownloadr.UseCases.WebPages.Download.DownloadWebPages;

namespace WebDownloadr.Web.WebPages;

/// <summary>
/// API endpoint for starting multiple downloads in a single request.
/// </summary>
public class DownloadBatch(IMediator _mediator) : Endpoint<DownloadWebPagesRequest, IEnumerable<Guid>>
{ 
  /// <inheritdoc />
  public override void Configure()
  {
    Post(DownloadWebPagesRequest.Route);
    AllowAnonymous();
  }

  /// <inheritdoc />
  public override async Task HandleAsync(DownloadWebPagesRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DownloadWebPagesCommand(request.Ids), cancellationToken);

    var successful = result.Where(r => r.IsSuccess).Select(r => r.Value);
    Response = successful.ToList();
  }
}
