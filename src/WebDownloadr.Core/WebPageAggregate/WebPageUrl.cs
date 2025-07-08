using Vogen;

namespace WebDownloadr.Core.WebPageAggregate;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
/// <summary>
/// Value object representing a valid HTTP or HTTPS URL.
/// </summary>
public partial struct WebPageUrl
{
  private static Validation Validate(in string url) =>
    !IsValidUrl(url) ? Validation.Invalid("Url must be valid") : Validation.Ok;

  private static bool IsValidUrl(string url)
  {
    if (string.IsNullOrWhiteSpace(url))
      return false;

    if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
    {
      // Check for HTTP or HTTPS
      return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
    }

    return false;
  }
}
