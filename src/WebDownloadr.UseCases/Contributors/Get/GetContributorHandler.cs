using WebDownloadr.Core.ContributorAggregate;
using WebDownloadr.Core.ContributorAggregate.Specifications;

namespace WebDownloadr.UseCases.Contributors.Get;

/// <summary>
/// Handles <see cref="GetContributorQuery"/> by loading the contributor from the repository.
/// </summary>
public class GetContributorHandler(IReadRepository<Contributor> _repository)
  : IQueryHandler<GetContributorQuery, Result<ContributorDTO>>
{
  public async Task<Result<ContributorDTO>> Handle(GetContributorQuery request, CancellationToken cancellationToken)
  {
    var spec = new ContributorByIdSpec(request.ContributorId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new ContributorDTO(entity.Id, entity.Name, entity.PhoneNumber?.Number ?? "");
  }
}
