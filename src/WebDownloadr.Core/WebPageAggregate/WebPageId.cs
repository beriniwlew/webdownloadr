using Vogen;

[assembly: VogenDefaults(
  staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]

namespace WebDownloadr.Core.WebPageAggregate;

/// <summary>
/// Strongly typed identifier for a <see cref="WebPage"/>.
/// </summary>
[ValueObject<Guid>]
public partial struct WebPageId;
