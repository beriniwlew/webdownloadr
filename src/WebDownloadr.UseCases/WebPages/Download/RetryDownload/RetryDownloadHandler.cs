using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.UseCases.WebPages.Download.RetryDownload;

public class RetryDownloadHandler(IDownloadWebPageService service)
  : ICommandHandler<RetryDownloadCommand, Result<Guid>>
{
  public Task<Result<Guid>> Handle(RetryDownloadCommand request,
    CancellationToken cancellationToken) =>
      service.RetryDownloadAsync(request.Id, cancellationToken);
}
