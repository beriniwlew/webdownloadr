namespace WebDownloadr.UseCases.WebPages.List;

/// <summary>
/// Query to list tracked pages with optional paging.
/// </summary>
/// <param name="Skip">Number of records to skip.</param>
/// <param name="Take">Maximum number of records to return.</param>
public record ListWebPagesQuery(int? Skip, int? Take) : IQuery<Result<IEnumerable<WebPageDTO>>>;
