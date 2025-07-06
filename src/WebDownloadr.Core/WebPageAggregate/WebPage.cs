namespace WebDownloadr.Core.WebPageAggregate;

public class WebPage : EntityBase<WebPage, WebPageId>, IAggregateRoot
{
  public WebPageUrl Url { get; private set; }
  public DownloadStatus Status { get; private set; } = DownloadStatus.NotSet;

  public WebPage(WebPageUrl url)
  {
    Url = url;
  }
  
  public void UpdateStatus(DownloadStatus status) => Status = status;
}
