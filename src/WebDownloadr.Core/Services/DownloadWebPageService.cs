using System.Collections.Concurrent;
using WebDownloadr.Core.Interfaces;
using WebDownloadr.Core.WebPageAggregate;
using WebDownloadr.Core.WebPageAggregate.Events;

namespace WebDownloadr.Core.Services;

/// <summary>
/// Implements <see cref="IDownloadWebPageService"/> using an <see cref="IWebPageDownloader"/>
/// and a repository for persisting <see cref="WebPage"/> entities.
/// </summary>
public class DownloadWebPageService(
  IRepository<WebPage> repository,
  IWebPageDownloader downloader,
  IMediator mediator) : IDownloadWebPageService
{
  private const string OutputDir = "downloads";
  private static readonly ConcurrentDictionary<Guid, CancellationTokenSource> _activeDownloads = new();

  /// <inheritdoc />
  public async Task<Result<Guid>> DownloadWebPageAsync(Guid id, CancellationToken cancellationToken)
  {
    var webPage = await repository.GetByIdAsync(id, cancellationToken);
    if (webPage is null)
    {
      return Result.NotFound();
    }

    webPage.UpdateStatus(DownloadStatus.DownloadInProgress);
    await repository.UpdateAsync(webPage, cancellationToken);

    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    _activeDownloads[webPage.Id.Value] = linkedCts;
    try
    {
      var result = await downloader.DownloadWebPagesAsync(new[] { webPage.Url.Value }, OutputDir, linkedCts.Token);

      if (result.IsSuccess && !linkedCts.IsCancellationRequested)
      {
        webPage.UpdateStatus(DownloadStatus.DownloadCompleted);
        await repository.UpdateAsync(webPage, cancellationToken);

        var filePath = Path.Combine(OutputDir, GetSafeFilename(webPage.Url.Value) + ".html");
        var content = await File.ReadAllTextAsync(filePath, cancellationToken);
        await mediator.Publish(new WebPageDownloadedEvent(webPage.Id.Value, content), cancellationToken);

        return Result.Success(webPage.Id.Value);
      }
      if (linkedCts.IsCancellationRequested)
      {
        webPage.UpdateStatus(DownloadStatus.DownloadCancelled);
        await repository.UpdateAsync(webPage, cancellationToken);
        return Result.Success(webPage.Id.Value);
      }

      webPage.UpdateStatus(DownloadStatus.DownloadError);
      await repository.UpdateAsync(webPage, cancellationToken);

      return Result<Guid>.Error(string.Join("; ", result.Errors));
    }
    catch (OperationCanceledException)
    {
      webPage.UpdateStatus(DownloadStatus.DownloadCancelled);
      await repository.UpdateAsync(webPage, cancellationToken);
      return Result.Success(webPage.Id.Value);
    }
    finally
    {
      _activeDownloads.TryRemove(webPage.Id.Value, out _);
    }
  }

  /// <inheritdoc />
  public async Task<IEnumerable<Result<Guid>>> DownloadWebPagesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
  {
    var results = new List<Result<Guid>>();
    foreach (var id in ids)
    {
      results.Add(await DownloadWebPageAsync(id, cancellationToken));
    }
    return results;
  }

  /// <inheritdoc />
  public Task<Result<Guid>> RetryDownloadAsync(Guid id, CancellationToken cancellationToken) =>
    DownloadWebPageAsync(id, cancellationToken);

  /// <inheritdoc />
  public async Task<Result<Guid>> CancelDownloadAsync(Guid id, CancellationToken cancellationToken)
  {
    var webPage = await repository.GetByIdAsync(id, cancellationToken);
    if (webPage is null)
    {
      return Result.NotFound();
    }

    if (_activeDownloads.TryRemove(id, out var cts))
    {
      cts.Cancel();
    }

    webPage.UpdateStatus(DownloadStatus.DownloadCancelled);
    await repository.UpdateAsync(webPage, cancellationToken);

    return Result.Success(webPage.Id.Value);
  }

  /// <summary>
  /// Replaces invalid file name characters so the URL can be used as a file name.
  /// </summary>
  /// <param name="url">The original page URL.</param>
  /// <returns>A sanitized string safe for use as a file name.</returns>
  private static string GetSafeFilename(string url)
  {
    foreach (var c in Path.GetInvalidFileNameChars())
    {
      url = url.Replace(c, '_');
    }

    return url.Replace("https://", "").Replace("http://", "").Replace("/", "_");
  }
}
