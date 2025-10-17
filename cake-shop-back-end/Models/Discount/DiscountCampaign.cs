using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Discount;

public class DiscountCampaign : MasterCommonModel
{
    public Guid id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public DateTime start_date { get; set; }

    public DateTime end_date { get; set; }

    public bool? is_active { get; set; }

    public string discount_type { get; set; } = null!;

    public decimal discount_value { get; set; }

    public decimal? max_discount { get; set; }
}
