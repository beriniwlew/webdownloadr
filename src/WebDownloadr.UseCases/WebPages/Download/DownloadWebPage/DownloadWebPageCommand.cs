namespace WebDownloadr.UseCases.WebPages.Download.DownloadWebPage;

public record DownloadWebPageCommand(Guid Id) : ICommand<Result<Guid>>;
