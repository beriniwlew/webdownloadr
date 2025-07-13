using System;

namespace WebDownloadr.Infrastructure.Data;

public class DownloadedPage
{
  public Guid Id { get; set; }
  public Guid WebPageId { get; set; }
  public string Content { get; set; } = string.Empty;
}
