namespace WebDownloadr.Core.Interfaces;

public interface IDownloadWebPageService
{
  Task<Result<Guid>> DownloadWebPageAsync(Guid id, CancellationToken cancellationToken);
  Task<IEnumerable<Result<Guid>>> DownloadWebPagesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
  Task<Result<Guid>> RetryDownloadAsync(Guid id, CancellationToken cancellationToken);
  Task<Result<Guid>> CancelDownloadAsync(Guid id, CancellationToken cancellationToken);
}
