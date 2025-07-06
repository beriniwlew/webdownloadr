using FluentValidation;

namespace WebDownloadr.Web.WebPages;

public class CreateWebPageValidator : Validator<CreateWebPageRequest>
{
  public CreateWebPageValidator()
  {
    RuleFor(x => x.Url)
      .NotEmpty()
      .WithMessage("Url is required");
    
    // TODO: Proper URI validation
  }
}
