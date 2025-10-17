using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class OrderItem : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid order_id { get; set; }

    public Guid? product_id { get; set; }

    public Guid? variant_id { get; set; }

    public int quantity { get; set; }

    public decimal price { get; set; }

}
