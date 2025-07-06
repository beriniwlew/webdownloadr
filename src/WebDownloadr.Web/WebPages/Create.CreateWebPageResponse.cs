namespace WebDownloadr.Web.WebPages;

public class CreateWebPageResponse
{
  public CreateWebPageResponse(Guid id, string url)
  {
    Id = id;
    Url = url;
  }
  
  public Guid Id { get; set; }
  public string Url { get; set; }
}
