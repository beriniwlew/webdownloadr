using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using Ardalis.Result;
using WebDownloadr.Core.Interfaces;
using WebDownloadr.Core.Services;
using WebDownloadr.Infrastructure.Web;

namespace WebDownloadr.UnitTests.Core.Services;

public class DownloadWebPageService_DownloadWebPageAsync
{
  private readonly IRepository<WebPage> _repository = Substitute.For<IRepository<WebPage>>();
  private readonly IMediator _mediator = Substitute.For<IMediator>();

  [Fact]
  public async Task UpdatesStatusAndPublishesEventOnSuccess()
  {
    var fileSystem = new MockFileSystem();
    var registry = new ActiveDownloadRegistry();
    var downloader = new FakeDownloader(true, fileSystem);
    var service = new DownloadWebPageService(_repository, downloader, _mediator, fileSystem, registry);
    var page = new WebPage(WebPageUrl.From("https://example.com")) { Id = WebPageId.From(Guid.NewGuid()) };
    _repository.GetByIdAsync<Guid>(page.Id.Value, Arg.Any<CancellationToken>()).Returns(page);
    _repository.UpdateAsync(page, Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

    var result = await service.DownloadWebPageAsync(page.Id.Value, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    page.Status.ShouldBe(DownloadStatus.DownloadCompleted);
    var calls = _mediator.ReceivedCalls().ToList();
    calls.ShouldContain(c => c.GetMethodInfo().Name == "Publish");
    var published = calls.First(c => c.GetMethodInfo().Name == "Publish").GetArguments()[0]!;
    published.GetType().Name.ShouldBe("WebPageDownloadedEvent");
    var idProperty = published.GetType().GetProperty("Id")!;
    ((Guid)idProperty.GetValue(published)!).ShouldBe(page.Id.Value);
    var contentProperty = published.GetType().GetProperty("Content")!;
    ((string)contentProperty.GetValue(published)!).ShouldBe(FakeDownloader.Content);
    registry.TryRemove(page.Id.Value, out _).ShouldBeFalse();
  }

  [Fact]
  public async Task SetsErrorStatusOnFailure()
  {
    var fileSystem = new MockFileSystem();
    var registry = new ActiveDownloadRegistry();
    var downloader = new FakeDownloader(false, fileSystem);
    var service = new DownloadWebPageService(_repository, downloader, _mediator, fileSystem, registry);
    var page = new WebPage(WebPageUrl.From("https://example.com")) { Id = WebPageId.From(Guid.NewGuid()) };
    _repository.GetByIdAsync<Guid>(page.Id.Value, Arg.Any<CancellationToken>()).Returns(page);
    _repository.UpdateAsync(page, Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

    var result = await service.DownloadWebPageAsync(page.Id.Value, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    page.Status.ShouldBe(DownloadStatus.DownloadError);
    _mediator.ReceivedCalls().ShouldNotContain(c => c.GetMethodInfo().Name == "Publish");
    registry.TryRemove(page.Id.Value, out _).ShouldBeFalse();
  }

  private class FakeDownloader(bool succeed, IFileSystem fileSystem) : IWebPageDownloader
  {
    public const string Content = "<html>content</html>";
    private readonly bool _succeed = succeed;
    private readonly IFileSystem _fileSystem = fileSystem;

    public Task<Result> DownloadWebPagesAsync(IEnumerable<(Guid Id, string Url)> pages, string outputDir, CancellationToken cancellationToken)
    {
      _fileSystem.Directory.CreateDirectory(outputDir);
      var page = pages.First();
      var fileName = _fileSystem.Path.Combine(outputDir, $"{page.Id}.html");
      _fileSystem.File.WriteAllText(fileName, Content);
      return Task.FromResult(_succeed ? Result.Success() : Result.Error("fail"));
    }
  }
}
