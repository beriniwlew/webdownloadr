using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Create;

public record CreateWebPageCommand(WebPageUrl Url) : ICommand<Result<WebPageId>>;
