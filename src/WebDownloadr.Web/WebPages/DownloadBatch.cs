using WebDownloadr.UseCases.WebPages.Download.DownloadWebPages;

namespace WebDownloadr.Web.WebPages;

public class DownloadBatch(IMediator _mediator) : Endpoint<DownloadWebPagesRequest, IEnumerable<Guid>>
{
  public override void Configure()
  {
    Post(DownloadWebPagesRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DownloadWebPagesRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DownloadWebPagesCommand(request.Ids), cancellationToken);

    var successful = result.Where(r => r.IsSuccess).Select(r => r.Value);
    Response = successful.ToList();
  }
}
