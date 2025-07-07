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
    Summary(s =>
    {
      s.ExampleRequest = new CreateWebPageRequest { Url = "https://example.com" };
    });
  }

  public override async Task HandleAsync(CreateWebPageRequest request, CancellationToken cancellationToken)
  {
    var command = new CreateWebPageCommand(WebPageUrl.From(request.Url!));
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateWebPageResponse(result.Value.Value, request.Url!);
    }
  }
}
