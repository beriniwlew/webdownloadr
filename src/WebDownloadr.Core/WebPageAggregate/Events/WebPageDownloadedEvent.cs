namespace WebDownloadr.Core.WebPageAggregate.Events;

/// <summary>
/// Raised when a web page download completes successfully.
/// </summary>
internal sealed class WebPageDownloadedEvent(Guid id, string content) : DomainEventBase
{
  public Guid Id { get; init; } = id;
  public string Content { get; } = content;
}
