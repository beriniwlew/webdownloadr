namespace WebDownloadr.UseCases.WebPages.Download.RetryDownload;

public record RetryDownloadCommand(Guid Id) : ICommand<Result<Guid>>;
