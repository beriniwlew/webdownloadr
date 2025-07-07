using WebDownloadr.Core.WebPageAggregate;
using WebDownloadr.UseCases.WebPages.Create;

namespace WebDownloadr.Web.WebPages;

public class Create(IMediator _mediator)
  : Endpoint<CreateWebPageRequest, CreateWebPageResponse>
{
  public override void Configure()
  {
    Post(CreateWebPageRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateWebPageRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateWebPageCommand(WebPageUrl.From(request.Url!)), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateWebPageResponse(result.Value.Value, request.Url!);
    }
  }
}
