using WebDownloadr.Core.Interfaces;
using WebDownloadr.Core.Services;
using WebDownloadr.Infrastructure.Data;
using WebDownloadr.Infrastructure.Data.Queries;
using WebDownloadr.Infrastructure.Web;
using WebDownloadr.UseCases.Contributors.List;


namespace WebDownloadr.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    string? connectionString = config.GetConnectionString("SqliteConnection");
    Guard.Against.Null(connectionString);
    services.AddDbContext<AppDbContext>(options =>
     options.UseSqlite(connectionString));

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
      .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
      .AddScoped<IListContributorsQueryService, ListContributorsQueryService>()
      .AddScoped<IDeleteContributorService, DeleteContributorService>()
      .AddScoped<IWebPageDownloader, SimpleWebPageDownloader>()
      .AddScoped<IDownloadWebPageService, DownloadWebPageService>();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
