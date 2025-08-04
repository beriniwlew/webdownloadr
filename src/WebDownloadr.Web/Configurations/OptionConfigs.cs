using System.Collections.Generic;
using Ardalis.ListStartupServices;
using Microsoft.Extensions.DependencyInjection;
using WebDownloadr.Infrastructure.Email;
using WebDownloadr.Infrastructure.Web;

namespace WebDownloadr.Web.Configurations;

public static class OptionConfigs
{
  public static IServiceCollection AddOptionConfigs(this IServiceCollection services,
                                                    IConfiguration configuration,
                                                    WebApplicationBuilder builder)
  {
    services.Configure<MailserverConfiguration>(configuration.GetSection("Mailserver"))
    .Configure<SimpleWebPageDownloaderOptions>(configuration.GetSection("WebPageDownloader"))
    // Configure Web Behavior
    .Configure<CookiePolicyOptions>(options =>
    {
      options.CheckConsentNeeded = context => true;
      options.MinimumSameSitePolicy = SameSiteMode.None;
    });

    if (builder.Environment.IsDevelopment())
    {
      // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
      services.Configure<ServiceConfig>(config =>
      {
        config.Services = new List<ServiceDescriptor>(builder.Services);

        // optional - default path to view services is /listallservices - recommended to choose your own path
        config.Path = "/listservices";
      });
    }

    Log.Information("{Project} were configured", "Options");

    return services;
  }
}

