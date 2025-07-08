using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.UseCases.WebPages.Download.RetryDownload;

/// <summary>
/// Invokes a download retry when handling <see cref="RetryDownloadCommand"/>.
/// </summary>
public class RetryDownloadHandler(IDownloadWebPageService service)
  : ICommandHandler<RetryDownloadCommand, Result<Guid>>
{
  /// <inheritdoc />
  public Task<Result<Guid>> Handle(RetryDownloadCommand request,
    CancellationToken cancellationToken) =>
      service.RetryDownloadAsync(request.Id, cancellationToken);
}
