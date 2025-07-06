namespace WebDownloadr.Core.WebPageAggregate.Events;

internal sealed  class WebPageDownloadedEvent(Guid id, string content) : DomainEventBase
{
  public Guid Id { get; init; } = id;
  public string Content { get; } = content;
}
