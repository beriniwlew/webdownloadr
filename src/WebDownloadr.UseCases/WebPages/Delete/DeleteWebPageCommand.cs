namespace WebDownloadr.UseCases.WebPages.Delete;

/// <summary>
/// Command to delete a tracked web page.
/// </summary>
/// <param name="Id">Identifier of the page to remove.</param>
public record DeleteWebPageCommand(Guid Id) : ICommand<Result>;
