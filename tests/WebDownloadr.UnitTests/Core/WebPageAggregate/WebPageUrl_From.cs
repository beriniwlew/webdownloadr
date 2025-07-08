namespace WebDownloadr.UnitTests.Core.WebPageAggregate;

public class WebPageUrl_From
{
  [Fact]
  public void ThrowsGivenInvalidUrl()
  {
    Should.Throw<ValueObjectValidationException>(() => WebPageUrl.From("foo"));
  }

  [Fact]
  public void CreatesGivenValidHttpUrl()
  {
    var url = WebPageUrl.From("http://example.com");
    url.Value.ShouldBe("http://example.com");
  }

  [Fact]
  public void CreatesGivenValidHttpsUrl()
  {
    var url = WebPageUrl.From("https://example.com");
    url.Value.ShouldBe("https://example.com");
  }
}
