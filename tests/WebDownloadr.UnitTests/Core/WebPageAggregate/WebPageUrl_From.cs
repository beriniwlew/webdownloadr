namespace WebDownloadr.UnitTests.Core.WebPageAggregate;

public class WebPageUrl_From
{
  [Fact]
  public void ThrowsGivenInvalidUrl()
  {
    Should.Throw<ValueObjectValidationException>(() => WebPageUrl.From("foo"));
  }
}
