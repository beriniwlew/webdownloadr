namespace WebDownloadr.UnitTests.UseCases.WebPages;

public class DeleteWebPageHandlerHandle
{
  private readonly IRepository<WebPage> _repository = Substitute.For<IRepository<WebPage>>();
  private readonly DeleteWebPageHandler _handler;
  private readonly WebPageUrl _url = WebPageUrl.From("https://example.com");

  public DeleteWebPageHandlerHandle()
  {
    _handler = new DeleteWebPageHandler(_repository);
  }

  [Fact]
  public async Task ReturnsNotFoundGivenMissingEntity()
  {
    _repository.GetByIdAsync<Guid>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult((WebPage?)null));

    var result = await _handler.Handle(new DeleteWebPageCommand(Guid.NewGuid()), CancellationToken.None);

    result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
  }

  [Fact]
  public async Task DeletesEntityWhenFound()
  {
    var page = new WebPage(_url);
    _repository.GetByIdAsync<Guid>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<WebPage?>(page));

    var result = await _handler.Handle(new DeleteWebPageCommand(Guid.NewGuid()), CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    await _repository.Received().DeleteAsync(page, Arg.Any<CancellationToken>());
  }
}
