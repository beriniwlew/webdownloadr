using Vogen;

[assembly: VogenDefaults(
  staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]

namespace WebDownloadr.Core.WebPageAggregate;

[ValueObject<Guid>]
/// <summary>
/// Strongly typed identifier for a <see cref="WebPage"/>.
/// </summary>
public partial struct WebPageId;
