using Vogen;
using WebDownloadr.Core.WebPageAggregate;

namespace WebDownloadr.Infrastructure.Data.Config;

[EfCoreConverter<WebPageId>]
[EfCoreConverter<WebPageUrl>]
internal partial class VogenEfCoreConverters;
