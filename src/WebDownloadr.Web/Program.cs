using Microsoft.Extensions.DependencyInjection;
using WebDownloadr.Infrastructure.Data;
using WebDownloadr.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

builder.AddLoggerConfigs();

var appLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

builder.Services.AddOptionConfigs(builder.Configuration, appLogger, builder);
builder.Services.AddServiceConfigs(appLogger, builder);

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.ShortSchemaNames = true;
                });

builder.AddServiceDefaults();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
  await app.UseAppMiddlewareAndSeedDatabase(seeder);
}

app.Run();

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program { }

