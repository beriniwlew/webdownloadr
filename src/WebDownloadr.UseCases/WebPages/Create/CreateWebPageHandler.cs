using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Create;

/// <summary>
/// Handles <see cref="CreateWebPageCommand"/> by persisting a new web page.
/// </summary>
public class CreateWebPageHandler(IRepository<WebPage> repository) : ICommandHandler<CreateWebPageCommand, Result<WebPageId>>
{
  public async Task<Result<WebPageId>> Handle(CreateWebPageCommand request, CancellationToken cancellationToken)
  {
    var newWebPage = new WebPage(request.Url);

    var createdWebPage = await repository.AddAsync(newWebPage, cancellationToken);

    return createdWebPage.Id;
  }
}
