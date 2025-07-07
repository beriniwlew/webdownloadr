using FastEndpoints;
using FluentValidation;

namespace WebDownloadr.Web.WebPages;

public class DeleteWebPageValidator : Validator<DeleteWebPageRequest>
{
  public DeleteWebPageValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}
