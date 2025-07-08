namespace WebDownloadr.UseCases.WebPages.Download.DownloadWebPage;

/// <summary>
/// Command to start downloading a single web page.
/// </summary>
/// <param name="Id">Identifier of the page to download.</param>
public record DownloadWebPageCommand(Guid Id) : ICommand<Result<Guid>>;
