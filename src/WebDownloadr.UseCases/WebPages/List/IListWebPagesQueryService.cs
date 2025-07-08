namespace WebDownloadr.UseCases.WebPages.List;

/// <summary>
/// Service responsible for fetching web page data. Typically implemented in Infrastructure.
/// </summary>
public interface IListWebPagesQueryService
{
  Task<IEnumerable<WebPageDTO>> ListAsync();
}
