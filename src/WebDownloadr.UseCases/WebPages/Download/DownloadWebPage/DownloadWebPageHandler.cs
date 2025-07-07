using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.UseCases.WebPages.Download.DownloadWebPage;

public class DownloadWebPageHandler(IDownloadWebPageService service)
  : ICommandHandler<DownloadWebPageCommand, Result<Guid>>
{
  public Task<Result<Guid>> Handle(DownloadWebPageCommand request,
    CancellationToken cancellationToken) =>
      service.DownloadWebPageAsync(request.Id, cancellationToken);
}
