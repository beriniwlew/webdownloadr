namespace WebDownloadr.UseCases.Contributors.Update;

/// <summary>
/// Command to rename an existing contributor.
/// </summary>
/// <param name="ContributorId">Identifier of the contributor.</param>
/// <param name="NewName">New contributor name.</param>
public record UpdateContributorCommand(int ContributorId, string NewName) : ICommand<Result<ContributorDTO>>;
