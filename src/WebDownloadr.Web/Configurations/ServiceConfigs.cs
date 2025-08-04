using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using WebDownloadr.Core.Interfaces;
using WebDownloadr.Infrastructure;
using WebDownloadr.Infrastructure.Email;

namespace WebDownloadr.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, WebApplicationBuilder builder)
  {
    services.AddSingleton<IFileSystem, FileSystem>();

    services.AddInfrastructureServices(builder.Configuration)
            .AddMediatrConfigs();

    if (builder.Environment.IsDevelopment())
    {
      // Use a local test email server
      // See: https://ardalis.com/configuring-a-local-test-email-server/
      services.AddScoped<IEmailSender, MimeKitEmailSender>();

      // Otherwise use this:
      //builder.Services.AddScoped<IEmailSender, FakeEmailSender>();

    }
    else
    {
      services.AddScoped<IEmailSender, MimeKitEmailSender>();
    }

    Log.Information("{Project} services registered", "Mediatr and Email Sender");

    return services;
  }
}

