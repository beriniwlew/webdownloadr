using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.UseCases.WebPages.Download.DownloadWebPages;

/// <summary>
/// Handles <see cref="DownloadWebPagesCommand"/> by requesting downloads for each page.
/// </summary>
public class DownloadWebPagesHandler(IDownloadWebPageService service)
  : ICommandHandler<DownloadWebPagesCommand, IEnumerable<Result<Guid>>>
{
  /// <inheritdoc />
  public Task<IEnumerable<Result<Guid>>> Handle(DownloadWebPagesCommand request,
    CancellationToken cancellationToken) =>
      service.DownloadWebPagesAsync(request.Ids, cancellationToken);
}
