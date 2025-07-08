namespace WebDownloadr.UseCases.Contributors.List;

/// <summary>
/// Query to list contributors with optional paging.
/// </summary>
/// <param name="Skip">Number of records to skip.</param>
/// <param name="Take">Maximum number of records to return.</param>
public record ListContributorsQuery(int? Skip, int? Take) : IQuery<Result<IEnumerable<ContributorDTO>>>;
