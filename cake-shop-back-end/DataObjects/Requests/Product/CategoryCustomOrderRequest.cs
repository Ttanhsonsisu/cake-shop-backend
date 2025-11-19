using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Product;

public class CategoryCustomOrderRequest : PaggingRequest
{
    public int? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int? Status { get; set; }
    public int? Parent { get; set; }
    public string? Description { get; set; }
    public int? Orders { get; set; }
}
