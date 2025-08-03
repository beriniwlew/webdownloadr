using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using Flurl.Http.Testing;
using Microsoft.Extensions.Options;
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
    var fileSystem = new MockFileSystem();
    var downloader = new SimpleWebPageDownloader(logger, options, fileSystem);
    var outputDir = fileSystem.Path.Combine("temp", Guid.NewGuid().ToString());

    var id = Guid.NewGuid();
    await downloader.DownloadWebPagesAsync(new[] { (id, "https://example.com") }, outputDir, CancellationToken.None);

    var files = fileSystem.Directory.GetFiles(outputDir);
    files.Length.ShouldBe(1);
    var file = files[0];
    file.EndsWith($"{id}.html").ShouldBeTrue();
    (await fileSystem.File.ReadAllTextAsync(file)).ShouldBe("<html>content</html>");
  }

  [Fact]
  public async Task RetriesWhenRequestFails()
  {
    using var httpTest = new HttpTest();
    httpTest.RespondWith(status: 500);
    httpTest.RespondWith("success");

    var logger = Substitute.For<ILogger<SimpleWebPageDownloader>>();
    var options = Options.Create(new SimpleWebPageDownloaderOptions());
    var fileSystem = new MockFileSystem();
    var downloader = new SimpleWebPageDownloader(logger, options, fileSystem);
    var outputDir = fileSystem.Path.Combine("temp", Guid.NewGuid().ToString());

    await downloader.DownloadWebPagesAsync(new[] { (Guid.NewGuid(), "https://example.com") }, outputDir, CancellationToken.None);

    httpTest.CallLog.Count.ShouldBe(2);
  }
}
