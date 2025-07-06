namespace WebDownloadr.Core.WebPageAggregate;

public class DownloadStatus : SmartEnum<DownloadStatus>
{
  public static readonly DownloadStatus DownloadQueued = new(nameof(DownloadQueued), 1);
  public static readonly DownloadStatus DownloadInProgress = new(nameof(DownloadInProgress), 2);
  public static readonly DownloadStatus DownloadCompleted = new(nameof(DownloadCompleted), 3);
  public static readonly DownloadStatus DownloadCancelled = new(nameof(DownloadCancelled), 4);
  public static readonly DownloadStatus DownloadError = new(nameof(DownloadError), 5);
  public static readonly DownloadStatus NotSet = new(nameof(NotSet), 6);
  
  protected DownloadStatus(string name, int value) : base(name, value) { }
}
