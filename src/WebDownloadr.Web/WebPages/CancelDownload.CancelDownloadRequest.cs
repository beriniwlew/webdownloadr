namespace WebDownloadr.Web.WebPages;

/// <summary>
/// Request payload for cancelling a running download.
/// </summary>
public class CancelDownloadRequest
{ 
  /// <summary>API route for the endpoint.</summary>
  public const string Route = "/WebPages/{WebPageId:guid}/download/cancel";

  /// <summary>
  /// Builds the route with the supplied identifier.
  /// </summary>
  public static string BuildRoute(Guid id) => Route.Replace("{WebPageId:guid}", id.ToString());

  /// <summary>Identifier of the page whose download should be cancelled.</summary>
  public Guid WebPageId { get; set; }
}
