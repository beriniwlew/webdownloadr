namespace WebDownloadr.UseCases.Contributors.Get;

/// <summary>
/// Query to retrieve a single contributor.
/// </summary>
/// <param name="ContributorId">Identifier of the contributor.</param>
public record GetContributorQuery(int ContributorId) : IQuery<Result<ContributorDTO>>;
