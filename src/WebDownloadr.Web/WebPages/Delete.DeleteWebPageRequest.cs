namespace WebDownloadr.Web.WebPages;

public record DeleteWebPageRequest
{
  public const string Route = "/WebPages/{Id:guid}";
  public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

  public Guid Id { get; set; }
}
