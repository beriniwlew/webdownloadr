using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.UseCases.WebPages.Download.CancelDownload;

public class CancelDownloadHandler(IDownloadWebPageService service)
  : ICommandHandler<CancelDownloadCommand, Result<Guid>>
{
  public Task<Result<Guid>> Handle(CancelDownloadCommand request, CancellationToken cancellationToken) =>
    service.CancelDownloadAsync(request.Id, cancellationToken);
}
