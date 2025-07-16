using Ardalis.Result;
using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.Infrastructure.Web;
using Flurl.Http;
using Polly;

/// <summary>
/// Basic <see cref="IWebPageDownloader"/> implementation that retrieves pages
/// via HTTP and saves them to disk.
/// </summary>
public class SimpleWebPageDownloader(ILogger<SimpleWebPageDownloader> logger,
    IOptions<SimpleWebPageDownloaderOptions> options) : IWebPageDownloader
{
  private readonly ILogger<SimpleWebPageDownloader> _logger = logger;
  private readonly SimpleWebPageDownloaderOptions _options = options.Value;

  private IAsyncPolicy GetFlurlRetryPolicy() =>
    Policy
      .Handle<FlurlHttpException>()
      .Or<FlurlHttpTimeoutException>()
      .WaitAndRetryAsync(
        retryCount: _options.RetryCount,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
      );

  /// <inheritdoc />
  public async Task<Result> DownloadWebPagesAsync(
    IEnumerable<(Guid Id, string Url)> pages,
    string outputDir,
    CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(outputDir);
    var policy = GetFlurlRetryPolicy();

    using var semaphore = new SemaphoreSlim(_options.MaxConcurrentDownloads);
    var tasks = pages.Select(page => DownloadWithSemaphore(page, outputDir, policy, semaphore, cancellationToken));
    await Task.WhenAll(tasks);

    return Result.Success();
  }

  private async Task DownloadWithSemaphore((Guid Id, string Url) page,
    string outputDir,
    IAsyncPolicy policy,
    SemaphoreSlim semaphore,
    CancellationToken cancellationToken)
  {
    await semaphore.WaitAsync(cancellationToken);
    try
    {
      await DownloadSingleWebPageAsync(page.Id, page.Url, outputDir, policy, cancellationToken);
    }
    finally
    {
      semaphore.Release();
    }
  }

  /// <summary>
  /// Downloads a single web page using the provided retry <paramref name="policy"/>.
  /// </summary>
  /// <param name="url">Page URL to download.</param>
  /// <param name="outputDir">Output directory.</param>
  /// <param name="policy">Polly retry policy.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  private async Task<Result> DownloadSingleWebPageAsync(Guid id, string url, string outputDir,
    IAsyncPolicy policy,
    CancellationToken cancellationToken)
  {
    _logger.LogInformation("Downloading {Url} and saving to directory {OutputDir}", url, outputDir);

    var stream = await policy.ExecuteAsync(ct =>
      url.WithTimeout(_options.TimeoutSeconds).GetStreamAsync(cancellationToken: ct), cancellationToken);

    var fileName = Path.Combine(outputDir, $"{id}.html");
    await using var file = File.Create(fileName);
    await stream.CopyToAsync(file, cancellationToken);

    return Result.Success();
  }
}
