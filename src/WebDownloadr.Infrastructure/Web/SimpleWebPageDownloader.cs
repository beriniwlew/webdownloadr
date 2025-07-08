using Ardalis.Result;
using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.Infrastructure.Web;
using Flurl.Http;
using Polly;

/// <summary>
/// Basic <see cref="IWebPageDownloader"/> implementation that retrieves pages
/// via HTTP and saves them to disk.
/// </summary>
public class SimpleWebPageDownloader(ILogger<SimpleWebPageDownloader> logger) : IWebPageDownloader
{
  private static IAsyncPolicy GetFlurlRetryPolicy() =>
    Policy
      .Handle<FlurlHttpException>()
      .Or<FlurlHttpTimeoutException>()
      .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
      );

  /// <inheritdoc />
  public async Task<Result> DownloadWebPagesAsync(IEnumerable<string> urls, string outputDir,
    CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(outputDir);
    var policy = GetFlurlRetryPolicy();
    var tasks = new List<Task>();
    foreach (var url in urls)
    {
      tasks.Add(DownloadSingleWebPageAsync(url, outputDir, policy, cancellationToken));
    }
    await Task.WhenAll(tasks);

    return Result.Success();
  }

  /// <summary>
  /// Downloads a single web page using the provided retry <paramref name="policy"/>.
  /// </summary>
  /// <param name="url">Page URL to download.</param>
  /// <param name="outputDir">Output directory.</param>
  /// <param name="policy">Polly retry policy.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  private async Task<Result> DownloadSingleWebPageAsync(string url, string outputDir, IAsyncPolicy policy,
    CancellationToken cancellationToken)
  {
    logger.LogInformation("Downloading {Url} and saving to directory {OutputDir}", url, outputDir);

    // Use Flurl to build the request, and Polly to handle retries.
    var response = await policy.ExecuteAsync(ct =>
      url.WithTimeout(20).GetAsync(cancellationToken: ct), cancellationToken);

    var content = await response.GetStringAsync();

    var fileName = Path.Combine(outputDir, GetSafeFilename(url) + ".html");
    await File.WriteAllTextAsync(fileName, content, cancellationToken);

    return Result.Success();
  }
  
  /// <summary>
  /// Converts a URL into a safe file name.
  /// </summary>
  private string GetSafeFilename(string url)
  {
    // Basic sanitization; we may want to use a better method
    foreach (var c in Path.GetInvalidFileNameChars())
      url = url.Replace(c, '_');
    return url.Replace("https://", "").Replace("http://", "").Replace("/", "_");
  }
}
