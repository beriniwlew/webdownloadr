using Flurl.Http.Testing;
using WebDownloadr.Infrastructure.Web;

namespace WebDownloadr.UnitTests.Infrastructure.Web;

public class SimpleWebPageDownloaderTests
{
    [Fact]
    public async Task SavesDownloadedContentToFile()
    {
        using var httpTest = new HttpTest();
        httpTest.RespondWith("<html>content</html>");

        var logger = Substitute.For<ILogger<SimpleWebPageDownloader>>();
        var downloader = new SimpleWebPageDownloader(logger);
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            await downloader.DownloadWebPagesAsync(new[] { "https://example.com" }, outputDir);

            var files = Directory.GetFiles(outputDir);
            files.Length.ShouldBe(1);
            var file = files[0];
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
        var downloader = new SimpleWebPageDownloader(logger);
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            await downloader.DownloadWebPagesAsync(new[] { "https://example.com" }, outputDir);

            httpTest.CallLog.Count.ShouldBe(2);
        }
        finally
        {
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);
        }
    }
}
