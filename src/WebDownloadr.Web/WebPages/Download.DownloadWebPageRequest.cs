namespace WebDownloadr.Web.WebPages;

/// <summary>
/// Request payload for initiating a web page download.
/// </summary>
public class DownloadWebPageRequest
{ 
  /// <summary>API route for the endpoint.</summary>
  public const string Route = "/WebPages/{WebPageId:guid}/download"; 

  /// <summary>
  /// Builds the route with a specific identifier.
  /// </summary>
  public static string BuildRoute(Guid webPageId) => Route.Replace("{WebPageId:guid}", webPageId.ToString());

  /// <summary>Identifier of the page to download.</summary>
  public Guid WebPageId { get; set; }
}
