namespace WebDownloadr.Web.WebPages;

public class UpdateWebPageResponse(WebPageRecord webPage)
{
  public WebPageRecord WebPage { get; set; } = webPage;
}
