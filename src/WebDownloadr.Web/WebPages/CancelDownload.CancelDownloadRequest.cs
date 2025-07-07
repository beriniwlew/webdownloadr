namespace WebDownloadr.Web.WebPages;

public class CancelDownloadRequest
{
  public const string Route = "/WebPages/{WebPageId:guid}/download/cancel";
  public static string BuildRoute(Guid id) => Route.Replace("{WebPageId:guid}", id.ToString());

  public Guid WebPageId { get; set; }
}
