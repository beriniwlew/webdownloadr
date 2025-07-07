namespace WebDownloadr.UseCases.WebPages.Get;

public record GetWebPageQuery(Guid WebPageId) : IQuery<Result<WebPageDTO>>;
