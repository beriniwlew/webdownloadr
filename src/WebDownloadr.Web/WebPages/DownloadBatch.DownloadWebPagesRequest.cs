using System.ComponentModel.DataAnnotations;

namespace WebDownloadr.Web.WebPages;

public class DownloadWebPagesRequest
{
  public const string Route = "/WebPages/download";

  [Required]
  public IEnumerable<Guid> Ids { get; set; } = [];
}
