using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Product;

public class VariantRequest : PaggingRequest
{
    public Guid? Id { get; set; }

    public Guid? ProductId { get; set; }

    public string? Sku { get; set; } = null!;

    public decimal? Price { get; set; }

    public List<VariantAtributeValueRequest>? VariantAtributeValues { get; set; } = new();

}

public class VariantAtributeValueRequest
{
    public Guid AttributeId { get; set; }
    public Guid AttributeValueId { get; set; }
    public Guid? VarriantId { get; set; }
}
