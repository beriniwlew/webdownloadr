using FluentValidation;

namespace WebDownloadr.Web.WebPages;

public class CreateWebPageValidator : Validator<CreateWebPageRequest>
{
  public CreateWebPageValidator()
  {
    RuleFor(x => x.Url)
      .NotEmpty()
      .WithMessage("Url is required")
      .Must(BeAValidHttpUrl)
      .WithMessage("Url must be a valid HTTP or HTTPS address");
  }

  private static bool BeAValidHttpUrl(string? url)
  {
    if (string.IsNullOrWhiteSpace(url))
      return false;

    return Uri.TryCreate(url, UriKind.Absolute, out var uri)
      && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
  }
}
