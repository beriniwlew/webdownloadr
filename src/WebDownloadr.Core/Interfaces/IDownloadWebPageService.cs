namespace WebDownloadr.Core.Interfaces;

/// <summary>
/// Provides high level operations for downloading <see cref="WebPage" />
/// entities and managing their lifecycle.
/// </summary>
public interface IDownloadWebPageService
{
  /// <summary>
  /// Begins downloading a single web page identified by <paramref name="id" />.
  /// </summary>
  /// <param name="id">Identifier of the page to download.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  /// <returns>Result containing the page identifier if successful.</returns>
  Task<Result<Guid>> DownloadWebPageAsync(Guid id, CancellationToken cancellationToken);

  /// <summary>
  /// Starts downloads for multiple pages.
  /// </summary>
  /// <param name="ids">Identifiers of the pages to download.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  Task<IEnumerable<Result<Guid>>> DownloadWebPagesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

  /// <summary>
  /// Attempts to download a page again after a previous failure.
  /// </summary>
  /// <param name="id">Identifier of the page to retry.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  Task<Result<Guid>> RetryDownloadAsync(Guid id, CancellationToken cancellationToken);

  /// <summary>
  /// Cancels an in‑progress download.
  /// </summary>
  /// <param name="id">Identifier of the page being downloaded.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  Task<Result<Guid>> CancelDownloadAsync(Guid id, CancellationToken cancellationToken);
}
