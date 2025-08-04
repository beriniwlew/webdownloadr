using System;
using System.Collections.Concurrent;
using System.Threading;
using WebDownloadr.Core.Interfaces;

namespace WebDownloadr.Infrastructure.Web;

/// <summary>
/// Thread-safe registry tracking in-progress downloads.
/// </summary>
public class ActiveDownloadRegistry : IActiveDownloadRegistry
{
  private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _downloads = new();

  /// <inheritdoc />
  public void Register(Guid id, CancellationTokenSource cts) => _downloads[id] = cts;

  /// <inheritdoc />
  public bool TryRemove(Guid id, out CancellationTokenSource? cts) => _downloads.TryRemove(id, out cts);
}
