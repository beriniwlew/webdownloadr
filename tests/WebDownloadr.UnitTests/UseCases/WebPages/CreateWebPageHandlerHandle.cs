namespace WebDownloadr.UnitTests.UseCases.WebPages;

public class CreateWebPageHandlerHandle
{
  private readonly IRepository<WebPage> _repository = Substitute.For<IRepository<WebPage>>();
  private readonly WebPageUrl _url = WebPageUrl.From("https://example.com");
  private readonly CreateWebPageHandler _handler;

  public CreateWebPageHandlerHandle()
  {
    _handler = new CreateWebPageHandler(_repository);
  }

  [Fact]
  public async Task ReturnsSuccessGivenValidUrl()
  {
    _repository.AddAsync(Arg.Any<WebPage>(), Arg.Any<CancellationToken>())
      .Returns(call => Task.FromResult(call.Arg<WebPage>()));

    var result = await _handler.Handle(new CreateWebPageCommand(_url), CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    await _repository.Received().AddAsync(Arg.Any<WebPage>(), Arg.Any<CancellationToken>());
  }
}
