namespace WebDownloadr.UseCases.Contributors.Create;

/// <summary>
/// Command to create a new <see cref="Contributor"/>.
/// </summary>
/// <param name="Name">Name of the contributor.</param>
/// <param name="PhoneNumber">Optional phone number.</param>
public record CreateContributorCommand(string Name, string? PhoneNumber) : Ardalis.SharedKernel.ICommand<Result<int>>;
