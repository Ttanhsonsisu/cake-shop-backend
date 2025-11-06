using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.Models.CakeProduct;

namespace cake_shop_back_end.DataObjects.Requests.Product;

public class AttributeRequest : PaggingRequest
{
    public Guid? Id { get; set; }

    public string? Code { get; set; } = null!;

    public string? Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Status { get; set; }

    public int? Orders { get; set; }

    public bool? AllowMultipleValues { get; set; } = false!;

    public List<AttributeValue>? AttributeValues { get; set; } = new List<AttributeValue>();
}
