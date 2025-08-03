using Ardalis.ListStartupServices;
using WebDownloadr.Infrastructure.Data;

namespace WebDownloadr.Web.Configurations;

public static class MiddlewareConfig
{
  public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(this WebApplication app, IDatabaseSeeder seeder)
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
    }
    else
    {
      app.UseDefaultExceptionHandler(); // from FastEndpoints
      app.UseHsts();
    }

    app.UseFastEndpoints()
        .UseSwaggerGen(); // Includes AddFileServer and static files middleware

    app.UseHttpsRedirection(); // Note this will drop Authorization headers

    await seeder.SeedAsync();

    return app;
  }
}

