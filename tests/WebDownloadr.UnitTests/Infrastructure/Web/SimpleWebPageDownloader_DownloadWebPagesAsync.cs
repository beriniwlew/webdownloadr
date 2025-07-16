using System.Threading;
using Flurl.Http.Testing;
using WebDownloadr.Infrastructure.Web;

namespace WebDownloadr.UnitTests.Infrastructure.Web;

public class SimpleWebPageDownloader_DownloadWebPagesAsync
{
  [Fact]
  public async Task SavesDownloadedContentToFile()
  {
    using var httpTest = new HttpTest();
    httpTest.RespondWith("<html>content</html>");

    var logger = Substitute.For<ILogger<SimpleWebPageDownloader>>();
    var options = Options.Create(new SimpleWebPageDownloaderOptions());
    var downloader = new SimpleWebPageDownloader(logger, options);
    var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    try
    {
      var id = Guid.NewGuid();
      await downloader.DownloadWebPagesAsync(new[] { (id, "https://example.com") }, outputDir, CancellationToken.None);

      var files = Directory.GetFiles(outputDir);
      files.Length.ShouldBe(1);
      var file = files[0];
      file.EndsWith($"{id}.html").ShouldBeTrue();
      (await File.ReadAllTextAsync(file)).ShouldBe("<html>content</html>");
    }
    finally
    {
      if (Directory.Exists(outputDir))
        Directory.Delete(outputDir, true);
    }
  }

  [Fact]
  public async Task RetriesWhenRequestFails()
  {
    using var httpTest = new HttpTest();
    httpTest.RespondWith(status: 500);
    httpTest.RespondWith("success");

    var logger = Substitute.For<ILogger<SimpleWebPageDownloader>>();
    var options = Options.Create(new SimpleWebPageDownloaderOptions());
    var downloader = new SimpleWebPageDownloader(logger, options);
    var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    try
    {
      await downloader.DownloadWebPagesAsync(new[] { (Guid.NewGuid(), "https://example.com") }, outputDir, CancellationToken.None);

      httpTest.CallLog.Count.ShouldBe(2);
    }
    finally
    {
      if (Directory.Exists(outputDir))
        Directory.Delete(outputDir, true);
    }
  }
}
