namespace WebDownloadr.Infrastructure.Web;

public class SimpleWebPageDownloaderOptions
{
    public int RetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 20;
    public int MaxConcurrentDownloads { get; set; } = 4;
}
