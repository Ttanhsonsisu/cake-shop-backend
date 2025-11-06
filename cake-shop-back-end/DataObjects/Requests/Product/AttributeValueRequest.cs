using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Product;

public class AttributeValueRequest : PaggingRequest
{
    public Guid? Id { get; set; }
    public Guid? AttributeId { get; set; }
    public string? Value { get; set; } = null!;
    public string? Name { get; set; } = null!;
}
