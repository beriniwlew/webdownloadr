namespace WebDownloadr.UseCases.Contributors.List;

/// <summary>
/// Handles <see cref="ListContributorsQuery"/> by returning data from the query service.
/// </summary>
public class ListContributorsHandler(IListContributorsQueryService _query)
  : IQueryHandler<ListContributorsQuery, Result<IEnumerable<ContributorDTO>>>
{
  public async Task<Result<IEnumerable<ContributorDTO>>> Handle(ListContributorsQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync();

    return Result.Success(result);
  }
}
