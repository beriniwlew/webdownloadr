using WebDownloadr.UseCases.WebPages.Delete;

namespace WebDownloadr.Web.WebPages;

public class Delete(IMediator _mediator) : Endpoint<DeleteWebPageRequest>
{
  public override void Configure()
  {
    Delete(DeleteWebPageRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteWebPageRequest request, CancellationToken cancellationToken)
  {
    var command = new DeleteWebPageCommand(request.Id);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
    }
  }
}
