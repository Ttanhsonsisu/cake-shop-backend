namespace cake_shop_back_end.DataObjects.Requests.Common;

public class FilterLoggingRequest : PaggingRequest
{
    
    public string? Search { get; set; }
    public string? Applications { get; set; }
    public string? Functions { get; set; }
    public string? Results { get; set; }
}
