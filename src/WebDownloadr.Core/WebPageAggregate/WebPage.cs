namespace WebDownloadr.Core.WebPageAggregate;

/// <summary>
/// Aggregate root representing a web page that can be downloaded.
/// </summary>
public class WebPage : EntityBase<WebPage, WebPageId>, IAggregateRoot
{
  /// <summary>
  /// The URL of the web page to download.
  /// </summary>
  public WebPageUrl Url { get; private set; }

  /// <summary>
  /// Current status of the download process.
  /// </summary>
  public DownloadStatus Status { get; private set; } = DownloadStatus.NotSet;

  /// <summary>
  /// Initializes a new instance of the <see cref="WebPage"/> class.
  /// </summary>
  /// <param name="url">Target web page URL.</param>
  public WebPage(WebPageUrl url)
  {
    Url = url;
  }

  /// <summary>
  /// Updates the current download status.
  /// </summary>
  /// <param name="status">The new status value.</param>
  public void UpdateStatus(DownloadStatus status) => Status = status;
}
