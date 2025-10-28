namespace cake_shop_back_end.DataObjects.Requests.Common;

public class LoggingRequest
{
    public string UserType { get; set; }
    public string Application { get; set; }
    public string Functions { get; set; }
    public string Actions { get; set; }
    public string IP { get; set; }
    public string Content { get; set; }
    public string ResultLogging { get; set; }
    public Boolean IsLogin { get; set; }
    public Boolean IsCallApi { get; set; }
    public string UserCreated { get; set; }
    public string? ApiName { get; set; }
    public DateTime? DateCreated { get; set; }
}
