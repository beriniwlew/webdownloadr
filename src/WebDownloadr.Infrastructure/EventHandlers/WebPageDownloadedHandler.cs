using MediatR;
using WebDownloadr.Core.WebPageAggregate.Events;
using WebDownloadr.Infrastructure.Data;

namespace WebDownloadr.Infrastructure.EventHandlers;

internal sealed class WebPageDownloadedHandler(AppDbContext db) : INotificationHandler<WebPageDownloadedEvent>
{
  private readonly AppDbContext _db = db;

  public async Task Handle(WebPageDownloadedEvent notification, CancellationToken cancellationToken)
  {
    var entity = new DownloadedPage
    {
      Id = Guid.NewGuid(),
      WebPageId = notification.Id,
      Content = notification.Content
    };

    _db.DownloadedPages.Add(entity);
    await _db.SaveChangesAsync(cancellationToken);
  }
}
