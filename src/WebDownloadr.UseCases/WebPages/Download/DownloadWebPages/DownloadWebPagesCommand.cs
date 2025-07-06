namespace WebDownloadr.UseCases.WebPages.Download.DownloadWebPages;

public record DownloadWebPagesCommand(IEnumerable<Guid> Ids) : ICommand<IEnumerable<Result<Guid>>>;
