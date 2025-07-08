using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages;

/// <summary>
/// Data transfer object used when returning web page information.
/// </summary>
/// <param name="Id">Identifier of the page.</param>
/// <param name="Url">Target URL.</param>
/// <param name="Status">Current download status.</param>
public record WebPageDTO(Guid Id, string Url, string Status);
