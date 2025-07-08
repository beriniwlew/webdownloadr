namespace WebDownloadr.UseCases.WebPages.Get;

/// <summary>
/// Query to fetch a single web page.
/// </summary>
/// <param name="WebPageId">Identifier of the page.</param>
public record GetWebPageQuery(Guid WebPageId) : IQuery<Result<WebPageDTO>>;
