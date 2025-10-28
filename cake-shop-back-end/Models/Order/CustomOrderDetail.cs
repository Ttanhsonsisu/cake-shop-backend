using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class CustomOrderDetail : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public Guid order_item_id { get; set; }
    public int custom_order_option_id { get; set; }
    public decimal? price_modify { get; set; }
}