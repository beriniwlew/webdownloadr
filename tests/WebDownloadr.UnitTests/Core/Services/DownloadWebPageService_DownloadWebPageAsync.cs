using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using Ardalis.Result;
using NSubstitute;
using Shouldly;
using WebDownloadr.Core.Interfaces;
using WebDownloadr.Core.Services;
using WebDownloadr.Core.WebPageAggregate;
using WebDownloadr.Core.WebPageAggregate.Events;

namespace WebDownloadr.UnitTests.Core.Services;

public class DownloadWebPageService_DownloadWebPageAsync
{
  private readonly IRepository<WebPage> _repository = Substitute.For<IRepository<WebPage>>();
  private readonly IMediator _mediator = Substitute.For<IMediator>();

  [Fact]
  public async Task UpdatesStatusAndPublishesEventOnSuccess()
  {
    var downloader = new FakeDownloader(true);
    var service = new DownloadWebPageService(_repository, downloader, _mediator);
    var page = new WebPage(WebPageUrl.From("https://example.com")) { Id = WebPageId.From(Guid.NewGuid()) };
    _repository.GetByIdAsync<Guid>(page.Id.Value, Arg.Any<CancellationToken>()).Returns(page);
    _repository.UpdateAsync(page, Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

    ClearActiveDownloads();
    try
    {
      var result = await service.DownloadWebPageAsync(page.Id.Value, CancellationToken.None);

      result.IsSuccess.ShouldBeTrue();
      page.Status.ShouldBe(DownloadStatus.DownloadCompleted);
      var calls = _mediator.ReceivedCalls().ToList();
      calls.ShouldContain(c => c.GetMethodInfo().Name == "Publish");
      var published = calls.First(c => c.GetMethodInfo().Name == "Publish").GetArguments()[0]!;
      published.GetType().Name.ShouldBe("WebPageDownloadedEvent");
      var idProperty = published.GetType().GetProperty("Id")!;
      var idProp = (Guid)idProperty.GetValue(published)!;
      idProp.ShouldBe(page.Id.Value);
      var contentProperty = published.GetType().GetProperty("Content")!;
      var contentProp = (string)contentProperty.GetValue(published)!;
      contentProp.ShouldBe(FakeDownloader.Content);
      IsDownloadActive(page.Id.Value).ShouldBeFalse();
    }
    finally
    {
      Cleanup();
    }
  }

  [Fact]
  public async Task SetsErrorStatusOnFailure()
  {
    var downloader = new FakeDownloader(false);
    var service = new DownloadWebPageService(_repository, downloader, _mediator);
    var page = new WebPage(WebPageUrl.From("https://example.com")) { Id = WebPageId.From(Guid.NewGuid()) };
    _repository.GetByIdAsync<Guid>(page.Id.Value, Arg.Any<CancellationToken>()).Returns(page);
    _repository.UpdateAsync(page, Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));

    ClearActiveDownloads();
    try
    {
      var result = await service.DownloadWebPageAsync(page.Id.Value, CancellationToken.None);

      result.IsSuccess.ShouldBeFalse();
      page.Status.ShouldBe(DownloadStatus.DownloadError);
      _mediator.ReceivedCalls().ShouldNotContain(c => c.GetMethodInfo().Name == "Publish");
      IsDownloadActive(page.Id.Value).ShouldBeFalse();
    }
    finally
    {
      Cleanup();
    }
  }

  private static bool IsDownloadActive(Guid id)
  {
    var dict = GetActiveDownloads();
    return dict.ContainsKey(id);
  }

  private static void ClearActiveDownloads()
  {
    GetActiveDownloads().Clear();
  }

  private static ConcurrentDictionary<Guid, CancellationTokenSource> GetActiveDownloads()
  {
    var field = typeof(DownloadWebPageService).GetField("_activeDownloads", BindingFlags.Static | BindingFlags.NonPublic)!;
    return (ConcurrentDictionary<Guid, CancellationTokenSource>)field.GetValue(null)!;
  }

  private static void Cleanup()
  {
    if (Directory.Exists("downloads"))
      Directory.Delete("downloads", true);
  }

  private class FakeDownloader(bool succeed) : IWebPageDownloader
  {
    public const string Content = "<html>content</html>";
    private readonly bool _succeed = succeed;

    public Task<Result> DownloadWebPagesAsync(IEnumerable<string> urls, string outputDir, CancellationToken cancellationToken)
    {
      Directory.CreateDirectory(outputDir);
      var url = urls.First();
      var fileName = Path.Combine(outputDir, GetSafeFilename(url) + ".html");
      File.WriteAllText(fileName, Content);
      return Task.FromResult(_succeed ? Result.Success() : Result.Error("fail"));
    }

    private static string GetSafeFilename(string url)
    {
      foreach (var c in Path.GetInvalidFileNameChars())
      {
        url = url.Replace(c, '_');
      }

      return url.Replace("https://", "").Replace("http://", "").Replace("/", "_");
    }
  }
}
