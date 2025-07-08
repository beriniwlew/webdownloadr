namespace WebDownloadr.UseCases.WebPages.Download.RetryDownload;

/// <summary>
/// Command to retry a failed web page download.
/// </summary>
/// <param name="Id">Identifier of the page to retry.</param>
public record RetryDownloadCommand(Guid Id) : ICommand<Result<Guid>>;
