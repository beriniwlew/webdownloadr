using Ardalis.Result;
using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.Infrastructure.Web;
using Flurl.Http;
using Polly;

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

  private async Task<Result> DownloadSingleWebPageAsync(string url, string outputDir, IAsyncPolicy policy,
    CancellationToken cancellationToken)
  {
    logger.LogInformation("Downloading {Url} and saving to directory {OutputDir}", url, outputDir);

    // Use Flurl to build the request, and Polly to handle retries.
    var response = await policy.ExecuteAsync(ct =>
      url.WithTimeout(20).GetAsync(ct), cancellationToken);

    var content = await response.GetStringAsync(cancellationToken);

    var fileName = Path.Combine(outputDir, GetSafeFilename(url) + ".html");
    await File.WriteAllTextAsync(fileName, content, cancellationToken);

    return Result.Success();
  }
  
  private string GetSafeFilename(string url)
  {
    // Basic sanitization; we may want to use a better method
    foreach (var c in Path.GetInvalidFileNameChars())
      url = url.Replace(c, '_');
    return url.Replace("https://", "").Replace("http://", "").Replace("/", "_");
  }
}
