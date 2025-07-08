namespace WebDownloadr.UseCases.Contributors;

/// <summary>
/// Data transfer object returned when listing or retrieving contributors.
/// </summary>
/// <param name="Id">Identifier of the contributor.</param>
/// <param name="Name">Contributor name.</param>
/// <param name="PhoneNumber">Optional phone number.</param>
public record ContributorDTO(int Id, string Name, string? PhoneNumber);
