using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Update;

public record UpdateWebPageCommand(WebPageId WebPageId, DownloadStatus NewStatus) : ICommand<Result<WebPageDTO>>
{
  
}
