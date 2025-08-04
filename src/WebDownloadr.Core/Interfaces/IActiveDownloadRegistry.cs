using System;
using System.Threading;
namespace WebDownloadr.Core.Interfaces;

/// <summary>
/// Tracks active web page downloads.
/// </summary>
public interface IActiveDownloadRegistry
{
  /// <summary>
  /// Registers a download operation for the specified <paramref name="id"/>.
  /// </summary>
  /// <param name="id">Identifier of the download.</param>
  /// <param name="cts">Cancellation source associated with the download.</param>
  void Register(Guid id, CancellationTokenSource cts);

  /// <summary>
  /// Attempts to remove a download and retrieve its cancellation source.
  /// </summary>
  /// <param name="id">Identifier of the download.</param>
  /// <param name="cts">Removed cancellation source.</param>
  /// <returns><c>true</c> if the download was tracked; otherwise <c>false</c>.</returns>
  bool TryRemove(Guid id, out CancellationTokenSource? cts);
}
