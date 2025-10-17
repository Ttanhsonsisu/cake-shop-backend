using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class Order : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid user_id { get; set; }

    public string order_code { get; set; } = null!;

    public decimal total_amount { get; set; }

    public decimal? discount_amount { get; set; }

    public int? payment_status { get; set; }

    public int? order_status { get; set; }

    public string? shipping_address { get; set; }

    public string? receiver_name { get; set; }

    public string? receiver_phone { get; set; }

    public string? note { get; set; }

    public string? payment_method { get; set; }

    public string? delivery_method { get; set; }
}
