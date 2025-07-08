namespace WebDownloadr.UseCases.Contributors.Delete;

/// <summary>
/// Command to remove a contributor from the system.
/// </summary>
/// <param name="ContributorId">Identifier of the contributor to delete.</param>
public record DeleteContributorCommand(int ContributorId) : ICommand<Result>;
