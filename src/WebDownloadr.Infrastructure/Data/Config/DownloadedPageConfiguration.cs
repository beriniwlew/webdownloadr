using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebDownloadr.Infrastructure.Data.Config;

public class DownloadedPageConfiguration : IEntityTypeConfiguration<DownloadedPage>
{
  public void Configure(EntityTypeBuilder<DownloadedPage> builder)
  {
    builder.Property(p => p.Id)
        .ValueGeneratedNever();
    builder.Property(p => p.Content)
        .IsRequired();
  }
}
