namespace WebDownloadr.Web.WebPages;

public class GetWebPageByIdRequest
{
  public const string Route = "/WebPages/{WebPageId:guid}";
  public static string BuildRoute(Guid webPageId) => Route.Replace("{WebPageId:guid}", webPageId.ToString());

  public Guid WebPageId { get; set; }
}
