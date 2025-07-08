using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.UseCases.WebPages.Download.CancelDownload;

/// <summary>
/// Handles <see cref="CancelDownloadCommand"/> by delegating to the download service.
/// </summary>
public class CancelDownloadHandler(IDownloadWebPageService service)
  : ICommandHandler<CancelDownloadCommand, Result<Guid>>
  {
  /// <inheritdoc />
  public Task<Result<Guid>> Handle(CancelDownloadCommand request, CancellationToken cancellationToken) =>
    service.CancelDownloadAsync(request.Id, cancellationToken);
  }
