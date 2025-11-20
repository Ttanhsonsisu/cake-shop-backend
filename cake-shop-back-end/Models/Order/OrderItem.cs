using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class OrderItem : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public Guid order_id { get; set; }
    public Guid? product_id { get; set; }
    public Guid? variant_id { get; set; }
    public int quantity { get; set; } = 1;
    public decimal price { get; set; }
    public decimal total_price { get; set; }
    // add
    public string? product_name { get; set; } = string.Empty;
    public string? variant_name { get;set; } = string.Empty;
}