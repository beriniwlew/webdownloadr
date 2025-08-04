using Serilog;
using WebDownloadr.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
  config.ReadFrom.Configuration(context.Configuration));

Log.Information("Starting web host");

builder.Services.AddOptionConfigs(builder.Configuration, builder);
builder.Services.AddServiceConfigs(builder);

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.ShortSchemaNames = true;
                });

builder.AddServiceDefaults();

var app = builder.Build();

await app.UseAppMiddlewareAndSeedDatabase();

app.Run();

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program { }

