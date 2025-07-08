namespace WebDownloadr.UseCases.WebPages.Download.DownloadWebPages;

/// <summary>
/// Command to download multiple web pages in a batch.
/// </summary>
/// <param name="Ids">Identifiers of the pages to download.</param>
public record DownloadWebPagesCommand(IEnumerable<Guid> Ids) : ICommand<IEnumerable<Result<Guid>>>;
