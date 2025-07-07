namespace WebDownloadr.Web.WebPages;

public class GetWebPageByIdRequest
{
  public const string Route = "/WebPages/{WebPageId:guid}";
  public static string BuildRoute(Guid id) => Route.Replace("{WebPageId:guid}", id.ToString());

  public Guid WebPageId { get; set; }
}
