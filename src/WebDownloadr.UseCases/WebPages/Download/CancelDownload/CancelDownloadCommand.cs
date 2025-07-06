namespace WebDownloadr.UseCases.WebPages.Download.CancelDownload;

public record CancelDownloadCommand(Guid Id) : ICommand<Result<Guid>>;
