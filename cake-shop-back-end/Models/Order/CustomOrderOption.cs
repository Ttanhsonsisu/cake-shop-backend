using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class CustomOrderOption : MasterCommonModel
{
    public int id { get; set; }
    public string? img_url { get; set; }
    public int? category_id { get; set; }
    public decimal? price_modify { get; set; }
    public string name { get; set; }
    public bool? is_active { get; set; } = true;
    public string? description { get; set; }
    public int? orders { get; set; }
}
