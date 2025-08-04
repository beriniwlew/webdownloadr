using System.IO.Abstractions;
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
  IMediator mediator,
  IFileSystem fileSystem,
  IActiveDownloadRegistry registry) : IDownloadWebPageService
{
  private const string OutputDir = "downloads";

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
    registry.Register(webPage.Id.Value, linkedCts);
    try
    {
      var pages = new[] { (webPage.Id.Value, webPage.Url.Value) };
      var result = await downloader.DownloadWebPagesAsync(pages, OutputDir, linkedCts.Token);

      if (result.IsSuccess && !linkedCts.IsCancellationRequested)
      {
        webPage.UpdateStatus(DownloadStatus.DownloadCompleted);
        await repository.UpdateAsync(webPage, linkedCts.Token);

        var filePath = fileSystem.Path.Combine(OutputDir, $"{webPage.Id.Value}.html");
        var content = await fileSystem.File.ReadAllTextAsync(filePath, linkedCts.Token);
        await mediator.Publish(new WebPageDownloadedEvent(webPage.Id.Value, content), linkedCts.Token);

        return Result.Success(webPage.Id.Value);
      }
      if (linkedCts.IsCancellationRequested)
      {
        webPage.UpdateStatus(DownloadStatus.DownloadCancelled);
        await repository.UpdateAsync(webPage, linkedCts.Token);
        return Result.Success(webPage.Id.Value);
      }

      webPage.UpdateStatus(DownloadStatus.DownloadError);
      await repository.UpdateAsync(webPage, linkedCts.Token);

      return Result<Guid>.Error(string.Join("; ", result.Errors));
    }
    catch (OperationCanceledException)
    {
      webPage.UpdateStatus(DownloadStatus.DownloadCancelled);
      await repository.UpdateAsync(webPage, linkedCts.Token);
      return Result.Success(webPage.Id.Value);
    }
    finally
    {
      registry.TryRemove(webPage.Id.Value, out _);
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

    if (registry.TryRemove(id, out var cts))
    {
      await cts!.CancelAsync();
    }

    webPage.UpdateStatus(DownloadStatus.DownloadCancelled);
    await repository.UpdateAsync(webPage, cancellationToken);

    return Result.Success(webPage.Id.Value);
  }
}
