using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.CakeProduct;

public class ProductCake : MasterCommonModel
{
    public Guid id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public decimal? base_price { get; set; }

    public string? SKU { get; set; }

    public int category_id { get; set; }
}
