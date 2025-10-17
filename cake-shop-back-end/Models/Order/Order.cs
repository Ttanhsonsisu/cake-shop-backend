using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class Order : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid user_id { get; set; }

    public string? recipient_name { get; set; }

    public string? recipient_phone { get; set; }

    public string? recipient_address { get; set; }

    public string? delivery_type { get; set; }

    public DateTime? delivery_time { get; set; }

    public decimal? delivery_fee { get; set; }

    public decimal total_amount { get; set; }

    public string? payment_method { get; set; }

    public string? order_status { get; set; }

}
