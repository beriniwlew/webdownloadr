namespace WebDownloadr.Core.WebPageAggregate;

/// <summary>
/// Enumerates the possible states of a web page download.
/// Uses <see cref="SmartEnum{TEnum}"/> for type-safe values.
/// </summary>
public class DownloadStatus : SmartEnum<DownloadStatus>
{
  /// <summary>Initial state when the download has been requested but not started.</summary>
  public static readonly DownloadStatus DownloadQueued = new(nameof(DownloadQueued), 1);

  /// <summary>The download is currently in progress.</summary>
  public static readonly DownloadStatus DownloadInProgress = new(nameof(DownloadInProgress), 2);

  /// <summary>The download completed successfully.</summary>
  public static readonly DownloadStatus DownloadCompleted = new(nameof(DownloadCompleted), 3);

  /// <summary>The download was cancelled.</summary>
  public static readonly DownloadStatus DownloadCancelled = new(nameof(DownloadCancelled), 4);

  /// <summary>An error occurred while downloading.</summary>
  public static readonly DownloadStatus DownloadError = new(nameof(DownloadError), 5);

  /// <summary>Status has not been set.</summary>
  public static readonly DownloadStatus NotSet = new(nameof(NotSet), 6);
  
  protected DownloadStatus(string name, int value) : base(name, value) { }
}
