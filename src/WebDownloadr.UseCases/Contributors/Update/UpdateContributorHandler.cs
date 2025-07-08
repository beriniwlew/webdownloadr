using WebDownloadr.Core.ContributorAggregate;

namespace WebDownloadr.UseCases.Contributors.Update;

/// <summary>
/// Handles <see cref="UpdateContributorCommand"/> by changing the contributor's name.
/// </summary>
public class UpdateContributorHandler(IRepository<Contributor> _repository)
  : ICommandHandler<UpdateContributorCommand, Result<ContributorDTO>>
{
  /// <summary>
  /// Updates an existing <see cref="Contributor"/>.
  /// </summary>
  /// <param name="request">Command containing the contributor ID and new name.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  /// <returns>Updated contributor data or <see cref="Result.NotFound"/>.</returns>
  public async Task<Result<ContributorDTO>> Handle(UpdateContributorCommand request, CancellationToken cancellationToken)
  {
    var existingContributor = await _repository.GetByIdAsync(request.ContributorId, cancellationToken);
    if (existingContributor == null)
    {
      return Result.NotFound();
    }

    existingContributor.UpdateName(request.NewName!);

    await _repository.UpdateAsync(existingContributor, cancellationToken);

    return new ContributorDTO(existingContributor.Id,
      existingContributor.Name, existingContributor.PhoneNumber?.Number ?? "");
  }
}
