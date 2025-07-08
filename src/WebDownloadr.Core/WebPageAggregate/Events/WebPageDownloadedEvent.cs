namespace WebDownloadr.Core.WebPageAggregate.Events;

/// <summary>
/// Raised when a web page download completes successfully.
/// </summary>
internal sealed class WebPageDownloadedEvent(Guid id, string content) : DomainEventBase
{
  /// <summary>
  /// Identifier of the downloaded page.
  /// </summary>
  public Guid Id { get; init; } = id;

  /// <summary>
  /// HTML content retrieved from the page.
  /// </summary>
  public string Content { get; } = content;
}
