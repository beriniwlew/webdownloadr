using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Create;

/// <summary>
/// Handles <see cref="CreateWebPageCommand"/> by persisting a new web page.
/// </summary>
public class CreateWebPageHandler(IRepository<WebPage> repository) : ICommandHandler<CreateWebPageCommand, Result<WebPageId>>
{
  /// <summary>
  /// Persists a new <see cref="WebPage"/> based on the provided URL.
  /// </summary>
  /// <param name="request">Command containing the page URL.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  /// <returns>Result with the identifier of the created page.</returns>
  public async Task<Result<WebPageId>> Handle(CreateWebPageCommand request, CancellationToken cancellationToken)
  {
    var newWebPage = new WebPage(request.Url);

    var createdWebPage = await repository.AddAsync(newWebPage, cancellationToken);

    return createdWebPage.Id;
  }
}
