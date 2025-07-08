using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Delete;

/// <summary>
/// Handles <see cref="DeleteWebPageCommand"/> by removing the entity from the repository.
/// </summary>
public class DeleteWebPageHandler : ICommandHandler<DeleteWebPageCommand, Result>
{

  private readonly IRepository<WebPage> _repository;

  public DeleteWebPageHandler(IRepository<WebPage> repository)
  {
    _repository = repository;
  }
  public async Task<Result> Handle(DeleteWebPageCommand request, CancellationToken cancellationToken)
  {
    var aggregateToDelete = await _repository.GetByIdAsync(request.Id, cancellationToken);

    if (aggregateToDelete == null)
    {
      return Result.NotFound();
    }

    await _repository.DeleteAsync(aggregateToDelete, cancellationToken);

    return Result.Success();
  }
}
