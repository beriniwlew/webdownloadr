using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UseCases.WebPages.Update;

public class UpdateWebPageHandler(IRepository<WebPage> repository)
 : ICommandHandler<UpdateWebPageCommand, Result<WebPageDTO>>
{
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
