namespace WebDownloadr.UseCases.WebPages.List;

public interface IListWebPagesQueryService
{
  Task<IEnumerable<WebPageDTO>> ListAsync();
}
