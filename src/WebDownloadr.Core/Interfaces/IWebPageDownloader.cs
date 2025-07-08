
namespace WebDownloadr.Core.Interfaces;

public interface IWebPageDownloader
{
  Task<Result> DownloadWebPagesAsync(IEnumerable<string> urls, string outputDir,
    CancellationToken cancellationToken);

}
