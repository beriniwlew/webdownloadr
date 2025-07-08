namespace WebDownloadr.Web.WebPages;

/// <summary>
/// Request payload for retrying a failed download.
/// </summary>
public class RetryDownloadRequest
{ 
  /// <summary>API route for the endpoint.</summary>
  public const string Route = "/WebPages/{WebPageId:guid}/download/retry"; 

  /// <summary>
  /// Builds the route with the supplied identifier.
  /// </summary>
  public static string BuildRoute(Guid id) => Route.Replace("{WebPageId:guid}", id.ToString());

  /// <summary>Identifier of the page to retry.</summary>
  public Guid WebPageId { get; set; }
}
