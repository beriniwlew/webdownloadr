using FastEndpoints;
using FluentValidation;

namespace WebDownloadr.Web.WebPages;

public class UpdateWebPageValidator : Validator<UpdateWebPageRequest>
{
  public UpdateWebPageValidator()
  {
    RuleFor(x => x.Status)
      .NotEmpty();

    RuleFor(x => x.WebPageId)
      .Must((req, id) => req.Id == id)
      .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
  }
}
