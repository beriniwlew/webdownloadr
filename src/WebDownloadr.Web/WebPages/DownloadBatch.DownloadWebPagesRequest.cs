using System.ComponentModel.DataAnnotations;

namespace WebDownloadr.Web.WebPages;

/// <summary>
/// Request payload for downloading multiple pages at once.
/// </summary>
public class DownloadWebPagesRequest
{ 
  /// <summary>API route for the endpoint.</summary>
  public const string Route = "/WebPages/download";

  /// <summary>
  /// Collection of page identifiers to download.
  /// </summary>
  [Required]
  public IEnumerable<Guid> Ids { get; set; } = [];
}
