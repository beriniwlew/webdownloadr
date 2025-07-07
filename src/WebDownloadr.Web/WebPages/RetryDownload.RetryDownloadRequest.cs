namespace WebDownloadr.Web.WebPages;

public class RetryDownloadRequest
{
  public const string Route = "/WebPages/{WebPageId:guid}/download/retry";
  public static string BuildRoute(Guid id) => Route.Replace("{WebPageId:guid}", id.ToString());

  public Guid WebPageId { get; set; }
}
