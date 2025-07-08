using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Create;

/// <summary>
/// Command to create a new <see cref="WebPage"/> from a URL.
/// </summary>
/// <param name="Url">Page address to track.</param>
public record CreateWebPageCommand(WebPageUrl Url) : ICommand<Result<WebPageId>>;
