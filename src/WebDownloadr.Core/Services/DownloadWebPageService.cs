using WebDownloadr.Core.Interfaces;
using WebDownloadr.Core.WebPageAggregate;
using WebDownloadr.Core.WebPageAggregate.Events;

namespace WebDownloadr.Core.Services;

public class DownloadWebPageService(
  IRepository<WebPage> repository,
  IWebPageDownloader downloader,
  IMediator mediator) : IDownloadWebPageService
{
  private const string OutputDir = "downloads";

  public async Task<Result<Guid>> DownloadWebPageAsync(Guid id, CancellationToken cancellationToken)
  {
    var webPage = await repository.GetByIdAsync(id, cancellationToken);
    if (webPage is null)
    {
      return Result.NotFound();
    }

    webPage.UpdateStatus(DownloadStatus.DownloadInProgress);
    await repository.UpdateAsync(webPage, cancellationToken);

    var result = await downloader.DownloadWebPagesAsync(new[] { webPage.Url.Value }, OutputDir);

    if (result.IsSuccess)
    {
      webPage.UpdateStatus(DownloadStatus.DownloadCompleted);
      await repository.UpdateAsync(webPage, cancellationToken);

      var filePath = Path.Combine(OutputDir, GetSafeFilename(webPage.Url.Value) + ".html");
      var content = await File.ReadAllTextAsync(filePath, cancellationToken);
      await mediator.Publish(new WebPageDownloadedEvent(webPage.Id.Value, content), cancellationToken);

      return Result.Success(webPage.Id.Value);
    }

    webPage.UpdateStatus(DownloadStatus.DownloadError);
    await repository.UpdateAsync(webPage, cancellationToken);

    return Result<Guid>.Error(string.Join("; ", result.Errors));
  }

  public async Task<IEnumerable<Result<Guid>>> DownloadWebPagesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
  {
    var results = new List<Result<Guid>>();
    foreach (var id in ids)
    {
      results.Add(await DownloadWebPageAsync(id, cancellationToken));
    }
    return results;
  }

  public Task<Result<Guid>> RetryDownloadAsync(Guid id, CancellationToken cancellationToken) =>
    DownloadWebPageAsync(id, cancellationToken);

  public async Task<Result<Guid>> CancelDownloadAsync(Guid id, CancellationToken cancellationToken)
  {
    var webPage = await repository.GetByIdAsync(id, cancellationToken);
    if (webPage is null)
    {
      return Result.NotFound();
    }

    webPage.UpdateStatus(DownloadStatus.DownloadCancelled);
    await repository.UpdateAsync(webPage, cancellationToken);

    return Result.Success(webPage.Id.Value);
  }

  private static string GetSafeFilename(string url)
  {
    foreach (var c in Path.GetInvalidFileNameChars())
    {
      url = url.Replace(c, '_');
    }

    return url.Replace("https://", "").Replace("http://", "").Replace("/", "_");
  }
}
