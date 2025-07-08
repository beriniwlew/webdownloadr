using Ardalis.SharedKernel;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WebDownloadr.Core.WebPageAggregate;
using WebDownloadr.Infrastructure.Data;

namespace WebDownloadr.IntegrationTests.Data;

public abstract class BaseEfRepoWebPageFixture
{
  protected AppDbContext _dbContext;

  protected BaseEfRepoWebPageFixture()
  {
    var options = CreateNewContextOptions();
    var dispatcher = Substitute.For<IDomainEventDispatcher>();
    _dbContext = new AppDbContext(options, dispatcher);
  }

  protected static DbContextOptions<AppDbContext> CreateNewContextOptions()
  {
    var serviceProvider = new ServiceCollection()
        .AddEntityFrameworkInMemoryDatabase()
        .BuildServiceProvider();

    var builder = new DbContextOptionsBuilder<AppDbContext>();
    builder.UseInMemoryDatabase("cleanarchitecture")
           .UseInternalServiceProvider(serviceProvider);

    return builder.Options;
  }

  protected EfRepository<WebPage> GetRepository()
  {
    return new EfRepository<WebPage>(_dbContext);
  }
}
