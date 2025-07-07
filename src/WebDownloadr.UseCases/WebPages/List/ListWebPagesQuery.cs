namespace WebDownloadr.UseCases.WebPages.List;

public record ListWebPagesQuery() : IQuery<Result<IEnumerable<WebPageDTO>>>;
