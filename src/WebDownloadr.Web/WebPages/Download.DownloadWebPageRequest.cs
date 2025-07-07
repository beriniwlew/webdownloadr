namespace WebDownloadr.Web.WebPages;

public class DownloadWebPageRequest
{
  public const string Route = "/WebPages/{WebPageId:guid}/download";
  public static string BuildRoute(Guid webPageId) => Route.Replace("{WebPageId:guid}", webPageId.ToString());

  public Guid WebPageId { get; set; }
}
