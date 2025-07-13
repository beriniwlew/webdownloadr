using System.IO;
using System.Linq;
using Ardalis.Result;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebDownloadr.Core.Interfaces;
using WebDownloadr.Infrastructure.Data;
using WebDownloadr.Infrastructure.Email;

namespace WebDownloadr.FunctionalTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
  protected override IHost CreateHost(IHostBuilder builder)
  {
    builder.UseEnvironment("Development");
    var host = builder.Build();
    host.Start();

    using (var scope = host.Services.CreateScope())
    {
      var scopedServices = scope.ServiceProvider;
      var db = scopedServices.GetRequiredService<AppDbContext>();
      var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

      db.Database.EnsureDeleted();
      db.Database.EnsureCreated();

      try
      {
        SeedData.PopulateTestDataAsync(db).Wait();
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {exceptionMessage}", ex.Message);
      }
    }

    return host;
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailSender));
      if (descriptor != null)
      {
        services.Remove(descriptor);
      }
      services.AddScoped<IEmailSender, FakeEmailSender>();

      var downloaderDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IWebPageDownloader));
      if (downloaderDescriptor != null)
      {
        services.Remove(downloaderDescriptor);
      }
      services.AddScoped<IWebPageDownloader, FakeDownloader>();
    });
  }

  private class FakeDownloader : IWebPageDownloader
  {
    public Task<Result> DownloadWebPagesAsync(IEnumerable<string> urls, string outputDir, CancellationToken cancellationToken)
    {
      Directory.CreateDirectory(outputDir);
      var url = urls.First();
      var fileName = Path.Combine(outputDir, GetSafeFilename(url) + ".html");
      File.WriteAllText(fileName, "<html></html>");
      return Task.FromResult(Result.Success());
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
