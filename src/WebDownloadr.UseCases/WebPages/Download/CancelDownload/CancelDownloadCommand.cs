namespace WebDownloadr.UseCases.WebPages.Download.CancelDownload;

/// <summary>
/// Command to cancel an in-progress web page download.
/// </summary>
/// <param name="Id">Identifier of the page being downloaded.</param>
public record CancelDownloadCommand(Guid Id) : ICommand<Result<Guid>>;
