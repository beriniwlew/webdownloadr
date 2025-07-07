namespace WebDownloadr.UseCases.WebPages.List;

public record ListWebPagesQuery(int? Skip, int? Take) : IQuery<Result<IEnumerable<WebPageDTO>>>;
