﻿
namespace WebDownloadr.Core.Interfaces;

/// <summary>
/// Downloads the raw content of web pages and saves them locally.
/// </summary>
public interface IWebPageDownloader
{
  /// <summary>
  /// Downloads a collection of URLs and writes each file to <paramref name="outputDir"/>.
  /// </summary>
  /// <param name="pages">Pairs of page identifiers and URLs.</param>
  /// <param name="outputDir">Directory where downloaded pages are saved.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  Task<Result> DownloadWebPagesAsync(IEnumerable<(Guid Id, string Url)> pages,
    string outputDir,
    CancellationToken cancellationToken);
}
