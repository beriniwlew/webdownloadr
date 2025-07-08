using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.Infrastructure.Data.Config;

public class WebPageConfiguration : IEntityTypeConfiguration<WebPage>
{
  public void Configure(EntityTypeBuilder<WebPage> builder)
  {
    builder.Property(p => p.Id)
      .HasValueGenerator<VogenSequentialGuidValueGenerator<WebPageId>>()
      .HasVogenConversion()
      .IsRequired();

    builder.Property(p => p.Url)
      .HasConversion(
        v => v.Value,
        v => WebPageUrl.From(v))
      .HasMaxLength(DataSchemaConstants.DEFAULT_URL_LENGTH)
      .IsRequired();

    builder.Property(x => x.Status)
      .HasConversion(
        x => x.Value,
        x => DownloadStatus.FromValue(x));
  }
}
