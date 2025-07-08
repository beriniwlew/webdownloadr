using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Update;

/// <summary>
/// Handles <see cref="UpdateWebPageCommand"/> by loading the entity and updating its status.
/// </summary>
public class UpdateWebPageHandler(IRepository<WebPage> repository)
 : ICommandHandler<UpdateWebPageCommand, Result<WebPageDTO>>
{
  /// <summary>
  /// Updates the status of an existing <see cref="WebPage"/>.
  /// </summary>
  /// <param name="request">Command with the page identifier and new status.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  /// <returns>Updated page data or <see cref="Result.NotFound"/>.</returns>
  public async Task<Result<WebPageDTO>> Handle(UpdateWebPageCommand request, CancellationToken cancellationToken)
  {
    var existingEntity = await repository.GetByIdAsync(request.WebPageId, cancellationToken);

    if (existingEntity == null)
    {
      return Result.NotFound();
    }

    existingEntity.UpdateStatus(request.NewStatus);

    await repository.UpdateAsync(existingEntity, cancellationToken);

    return new WebPageDTO(existingEntity.Id.Value, existingEntity.Url.Value, existingEntity.Status.ToString());
  }
}
