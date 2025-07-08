using WebDownloadr.Core.ContributorAggregate;

namespace WebDownloadr.UseCases.Contributors.Create;

/// <summary>
/// Handles <see cref="CreateContributorCommand"/> by persisting the new contributor.
/// </summary>
public class CreateContributorHandler(IRepository<Contributor> repository)
  : ICommandHandler<CreateContributorCommand, Result<int>>
{
  /// <summary>
  /// Creates a new <see cref="Contributor"/> in the repository.
  /// </summary>
  /// <param name="request">Command containing contributor details.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  /// <returns>Identifier of the newly created contributor.</returns>
  public async Task<Result<int>> Handle(CreateContributorCommand request,
    CancellationToken cancellationToken)
  {
    var newContributor = new Contributor(request.Name);

    if (!string.IsNullOrEmpty(request.PhoneNumber))
    {
      newContributor.SetPhoneNumber(request.PhoneNumber);
    }

    var createdItem = await repository.AddAsync(newContributor, cancellationToken);

    return createdItem.Id;
  }
}
