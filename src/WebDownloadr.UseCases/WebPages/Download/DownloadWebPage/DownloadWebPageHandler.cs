using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.UseCases.WebPages.Download.DownloadWebPage;

/// <summary>
/// Handles a <see cref="DownloadWebPageCommand"/> by invoking the download service.
/// </summary>
public class DownloadWebPageHandler(IDownloadWebPageService service)
  : ICommandHandler<DownloadWebPageCommand, Result<Guid>>
{
  /// <inheritdoc />
  public Task<Result<Guid>> Handle(DownloadWebPageCommand request,
    CancellationToken cancellationToken) =>
      service.DownloadWebPageAsync(request.Id, cancellationToken);
}
