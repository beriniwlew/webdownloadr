using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages;

public record WebPageDTO(Guid Id, string Url, string Status);
