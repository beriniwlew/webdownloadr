using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.UseCases.WebPages.Download.DownloadWebPages;

public class DownloadWebPagesHandler(IDownloadWebPageService service)
  : ICommandHandler<DownloadWebPagesCommand, IEnumerable<Result<Guid>>>
{
  public Task<IEnumerable<Result<Guid>>> Handle(DownloadWebPagesCommand request,
    CancellationToken cancellationToken) =>
      service.DownloadWebPagesAsync(request.Ids, cancellationToken);
}
