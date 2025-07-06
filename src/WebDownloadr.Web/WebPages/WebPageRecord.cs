using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.Web.WebPages;

public record WebPageRecord(Guid Id, string Url, DownloadStatus Status);
