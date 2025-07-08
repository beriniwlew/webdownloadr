using WebDownloadr.Web.WebPages;

namespace WebDownloadr.UnitTests.Web.WebPages;

public class CreateWebPageValidator_Validate
{
  private readonly CreateWebPageValidator _validator = new();

  [Fact]
  public void ReturnsValidGivenHttpUrl()
  {
    var request = new CreateWebPageRequest { Url = "https://example.com" };

    var result = _validator.Validate(request);

    result.IsValid.ShouldBeTrue();
  }

  [Fact]
  public void ReturnsInvalidGivenBadUrl()
  {
    var request = new CreateWebPageRequest { Url = "ftp://example.com" };

    var result = _validator.Validate(request);

    result.IsValid.ShouldBeFalse();
    result.Errors.ShouldContain(e => e.ErrorMessage == "Url must be a valid HTTP or HTTPS address");
  }
}
