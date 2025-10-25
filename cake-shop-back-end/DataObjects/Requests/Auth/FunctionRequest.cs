using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Auth;

public class FunctionRequest : PaggingRequest
{
    public Guid? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int? Status { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public int? ApplicationId { get; set; }
    public Boolean? IsDefault { get; set; }
}
