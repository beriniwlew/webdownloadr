using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using WebDownloadr.Core.Interfaces;
using WebDownloadr.Core.Services;
using WebDownloadr.Infrastructure.Web;

namespace WebDownloadr.UnitTests.Core.Services;

public class DownloadWebPageService_CancelDownloadAsync
{
  private readonly IRepository<WebPage> _repository = Substitute.For<IRepository<WebPage>>();
  private readonly IMediator _mediator = Substitute.For<IMediator>();
  private readonly MockFileSystem _fileSystem = new();
  private readonly ActiveDownloadRegistry _registry = new();
  private readonly FakeDownloader _downloader;
  private readonly DownloadWebPageService _service;

  public DownloadWebPageService_CancelDownloadAsync()
  {
    _downloader = new FakeDownloader(_fileSystem);
    _service = new DownloadWebPageService(_repository, _downloader, _mediator, _fileSystem, _registry);
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
    _registry.TryRemove(page.Id.Value, out _).ShouldBeFalse();
  }

  private class FakeDownloader : IWebPageDownloader
  {
    public TaskCompletionSource Started { get; } = new();
    private readonly IFileSystem _fileSystem;
    public FakeDownloader(IFileSystem fileSystem) => _fileSystem = fileSystem;

    public async Task<Result> DownloadWebPagesAsync(IEnumerable<(Guid Id, string Url)> pages, string outputDir, CancellationToken cancellationToken)
    {
      _fileSystem.Directory.CreateDirectory(outputDir);
      Started.SetResult();
      await Task.Delay(Timeout.Infinite, cancellationToken);
      return Result.Success();
    }
  }
}
