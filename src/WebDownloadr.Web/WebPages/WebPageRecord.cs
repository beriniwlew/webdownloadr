using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.Web.WebPages;

/// <summary>
/// Response record returned by API endpoints representing a web page.
/// </summary>
/// <param name="Id">Identifier of the page.</param>
/// <param name="Url">Target URL.</param>
/// <param name="Status">Current download status.</param>
public record WebPageRecord(Guid Id, string Url, DownloadStatus Status);
