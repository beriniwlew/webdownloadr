namespace WebDownloadr.UnitTests.UseCases.WebPages;

public class UpdateWebPageHandlerHandle
{
  private readonly IRepository<WebPage> _repository = Substitute.For<IRepository<WebPage>>();
  private readonly UpdateWebPageHandler _handler;
  private readonly WebPageUrl _url = WebPageUrl.From("https://example.com");

  public UpdateWebPageHandlerHandle()
  {
    _handler = new UpdateWebPageHandler(_repository);
  }

  [Fact]
  public async Task ReturnsNotFoundGivenMissingEntity()
  {
    _repository.GetByIdAsync<WebPageId>(WebPageId.From(Guid.NewGuid()), Arg.Any<CancellationToken>())
      .ReturnsForAnyArgs(Task.FromResult((WebPage?)null));

    var result = await _handler.Handle(
      new UpdateWebPageCommand(WebPageId.From(Guid.NewGuid()), DownloadStatus.DownloadCompleted),
      CancellationToken.None);

    result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
  }

  [Fact]
  public async Task ReturnsDtoWhenUpdateSucceeds()
  {
    var page = new WebPage(_url) { Id = WebPageId.From(Guid.NewGuid()) };
    _repository.GetByIdAsync<WebPageId>(WebPageId.From(Guid.NewGuid()), Arg.Any<CancellationToken>())
      .ReturnsForAnyArgs(Task.FromResult<WebPage?>(page));

    _repository.UpdateAsync(page, Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(1));

    var result = await _handler.Handle(
      new UpdateWebPageCommand(WebPageId.From(Guid.NewGuid()), DownloadStatus.DownloadCompleted),
      CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    await _repository.Received().UpdateAsync(page, Arg.Any<CancellationToken>());
  }
}
