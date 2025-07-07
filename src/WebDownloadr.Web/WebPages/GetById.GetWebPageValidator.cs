using FastEndpoints;
using FluentValidation;

namespace WebDownloadr.Web.WebPages;

public class GetWebPageValidator : Validator<GetWebPageByIdRequest>
{
  public GetWebPageValidator()
  {
    RuleFor(x => x.WebPageId)
      .NotEmpty();
  }
}
