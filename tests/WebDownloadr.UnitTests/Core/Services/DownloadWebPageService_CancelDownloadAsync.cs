using Ardalis.Result;
using NSubstitute;
using Shouldly;
using WebDownloadr.Core.Interfaces;
using WebDownloadr.Core.Services;
using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.UnitTests.Core.Services;

public class DownloadWebPageService_CancelDownloadAsync
{
  private readonly IRepository<WebPage> _repository = Substitute.For<IRepository<WebPage>>();
  private readonly IMediator _mediator = Substitute.For<IMediator>();
  private readonly FakeDownloader _downloader = new();
  private readonly DownloadWebPageService _service;

  public DownloadWebPageService_CancelDownloadAsync()
  {
    _service = new DownloadWebPageService(_repository, _downloader, _mediator);
  }

  [Fact]
  public async Task ReturnsNotFoundGivenMissingWebPage()
  {
    _repository.GetByIdAsync<Guid>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns(Task.FromResult((WebPage?)null));

    var result = await _service.CancelDownloadAsync(Guid.NewGuid(), CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
  }

  [Fact]
  public async Task CancelsRunningDownload()
  {
    var page = new WebPage(WebPageUrl.From("https://example.com")) { Id = WebPageId.From(Guid.NewGuid()) };
    _repository.GetByIdAsync<Guid>(page.Id.Value, Arg.Any<CancellationToken>())
        .Returns(page);
    _repository.UpdateAsync(page, Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

    var downloadTask = _service.DownloadWebPageAsync(page.Id.Value, CancellationToken.None);
    await _downloader.Started.Task;

    var cancelResult = await _service.CancelDownloadAsync(page.Id.Value, CancellationToken.None);
    cancelResult.IsSuccess.ShouldBeTrue();

    await downloadTask;

    page.Status.ShouldBe(DownloadStatus.DownloadCancelled);
  }

  private class FakeDownloader : IWebPageDownloader
  {
    public TaskCompletionSource Started { get; } = new();

    public async Task<Result> DownloadWebPagesAsync(IEnumerable<(Guid Id, string Url)> pages, string outputDir, CancellationToken cancellationToken)
    {
      Directory.CreateDirectory(outputDir);
      Started.SetResult();
      await Task.Delay(Timeout.Infinite, cancellationToken);
      return Result.Success();
    }
  }
}
