using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading;
using Ardalis.Result;
using MediatR;
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
  private readonly IFileSystem _fileSystem = fileSystem;
  private readonly IActiveDownloadRegistry _registry = registry;

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
    _registry.Register(webPage.Id.Value, linkedCts);
    try
    {
      var pages = new[] { (webPage.Id.Value, webPage.Url.Value) };
      var result = await downloader.DownloadWebPagesAsync(pages, OutputDir, linkedCts.Token);

      if (result.IsSuccess && !linkedCts.IsCancellationRequested)
      {
        webPage.UpdateStatus(DownloadStatus.DownloadCompleted);
        await repository.UpdateAsync(webPage, cancellationToken);

        var filePath = _fileSystem.Path.Combine(OutputDir, $"{webPage.Id.Value}.html");
        var content = await _fileSystem.File.ReadAllTextAsync(filePath, cancellationToken);
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
      _registry.TryRemove(webPage.Id.Value, out _);
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

    if (_registry.TryRemove(id, out var cts))
    {
      cts!.Cancel();
    }

    webPage.UpdateStatus(DownloadStatus.DownloadCancelled);
    await repository.UpdateAsync(webPage, cancellationToken);

    return Result.Success(webPage.Id.Value);
  }
}
