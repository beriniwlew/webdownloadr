using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Update;

/// <summary>
/// Command to change the download status of a web page.
/// </summary>
/// <param name="WebPageId">Identifier of the page.</param>
/// <param name="NewStatus">The new status value.</param>
public record UpdateWebPageCommand(WebPageId WebPageId, DownloadStatus NewStatus) : ICommand<Result<WebPageDTO>>
{

}
