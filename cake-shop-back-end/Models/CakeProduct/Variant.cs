using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.CakeProduct;

public class Variant : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid product_id { get; set; }

    public string sku { get; set; } = null!;

    public decimal price { get; set; }
}
