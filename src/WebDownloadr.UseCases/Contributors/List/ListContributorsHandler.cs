namespace WebDownloadr.UseCases.Contributors.List;

/// <summary>
/// Handles <see cref="ListContributorsQuery"/> by returning data from the query service.
/// </summary>
public class ListContributorsHandler(IListContributorsQueryService _query)
  : IQueryHandler<ListContributorsQuery, Result<IEnumerable<ContributorDTO>>>
{
  /// <summary>
  /// Retrieves a list of contributors.
  /// </summary>
  /// <param name="request">Query instance (unused).</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  /// <returns>Contributors returned by the query service.</returns>
  public async Task<Result<IEnumerable<ContributorDTO>>> Handle(ListContributorsQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync(cancellationToken);

    return Result.Success(result);
  }
}
