using System.ComponentModel.DataAnnotations;

namespace WebDownloadr.Web.WebPages;

public class CreateWebPageRequest
{
  public const string Route = "/WebPages";

  [Required]
  public string? Url { get; set; }
}
