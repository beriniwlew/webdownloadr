using System.ComponentModel.DataAnnotations;

namespace WebDownloadr.Web.WebPages;

public class UpdateWebPageRequest
{
  public const string Route = "/WebPages/{WebPageId:guid}";
  public static string BuildRoute(Guid webPageId) => Route.Replace("{WebPageId:guid}", webPageId.ToString());

  public Guid WebPageId { get; set; }

  [Required]
  public Guid Id { get; set; }

  [Required]
  public string? Status { get; set; }
}
