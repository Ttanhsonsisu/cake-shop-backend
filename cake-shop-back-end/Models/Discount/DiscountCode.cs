using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Discount;

public class DiscountCode : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public string name { get; set; }
    public string? description { get; set; }
    public DateTime start_date { get; set; }
    public DateTime end_date { get; set; }
    public bool is_active { get; set; } = true;
    public string discount_type { get; set; }
    public decimal discount_value { get; set; }
    public decimal? max_discount { get; set; }
}
