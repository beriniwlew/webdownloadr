using WebDownloadr.UseCases.WebPages.Get;
using WebDownloadr.UseCases.WebPages.Update;
using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.Web.WebPages;

public class Update(IMediator _mediator) : Endpoint<UpdateWebPageRequest, UpdateWebPageResponse>
{
  public override void Configure()
  {
    Put(UpdateWebPageRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateWebPageRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new UpdateWebPageCommand(WebPageId.From(request.Id), DownloadStatus.FromName(request.Status!)), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      var dto = result.Value;
      Response = new UpdateWebPageResponse(new WebPageRecord(dto.Id, dto.Url, dto.Status));
      return;
    }
  }
}
