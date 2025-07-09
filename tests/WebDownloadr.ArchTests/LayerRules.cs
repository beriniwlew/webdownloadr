using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace WebDownloadr.ArchTests;

public class LayerRules
{
  private static readonly Architecture Architecture = new ArchLoader()
      .LoadAssemblies(
          typeof(WebDownloadr.Core.ContributorAggregate.Contributor).Assembly,
          typeof(WebDownloadr.UseCases.WebPages.List.ListWebPagesHandler).Assembly,
          typeof(WebDownloadr.Infrastructure.InfrastructureServiceExtensions).Assembly,
          typeof(WebDownloadr.Web.Configurations.OptionConfigs).Assembly)
      .Build();

  private static readonly IObjectProvider<IType> Core = Types().That().ResideInNamespace("WebDownloadr.Core", true);
  private static readonly IObjectProvider<IType> UseCases = Types().That().ResideInNamespace("WebDownloadr.UseCases", true);
  private static readonly IObjectProvider<IType> Infrastructure = Types().That().ResideInNamespace("WebDownloadr.Infrastructure", true);
  private static readonly IObjectProvider<IType> Web = Types().That().ResideInNamespace("WebDownloadr.Web", true);
  private static readonly IObjectProvider<IType> Bcl = Types().That().ResideInNamespace("System", true);

  [Fact]
  public void Core_does_not_depend_on_other_layers() =>
      Types().That().Are(Core)
          .Should().NotDependOnAny(
              Types().That()
                  .ResideInNamespace("WebDownloadr.UseCases", true)
                  .Or().ResideInNamespace("WebDownloadr.Infrastructure", true)
                  .Or().ResideInNamespace("WebDownloadr.Web", true))
          .Check(Architecture);

  [Fact]
  public void UseCases_depend_only_on_Core_and_BCL() =>
      Types().That().Are(UseCases)
          .Should()
          .OnlyDependOn(
              Types().That()
                  .ResideInNamespace("WebDownloadr.Core", true)
                  .Or().ResideInNamespace("WebDownloadr.UseCases", true)
                  .Or().ResideInNamespace("System", true))
          .Check(Architecture);

  [Fact]
  public void Infrastructure_does_not_depend_on_Web() =>
      Types().That().Are(Infrastructure)
          .Should().NotDependOnAny(Web)
          .Check(Architecture);
}
