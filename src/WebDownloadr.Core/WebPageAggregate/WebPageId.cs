using Vogen;

[assembly: VogenDefaults(
  staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]

namespace WebDownloadr.Core.WebPageAggregate;

[ValueObject<Guid>]
public partial struct WebPageId;
