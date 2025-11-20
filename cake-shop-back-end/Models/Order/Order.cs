using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class Order : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public Guid user_id { get; set; }
    public string? recipient_name { get; set; }
    public string? recipient_phone { get; set; }
    public string? recipient_address { get; set; }
    public string? delivery_type { get; set; }
    public string? name_guest { get; set; }
    public string? phone_guest { get; set; }
    public string? email_guest { get; set; }
    public int? order_tag { get; set; }
    public DateTime? delivery_time { get; set; }
    public int order_source { get; set; }
    public decimal delivery_fee { get; set; } = 0m;
    public decimal total_amount { get; set; }
    public int payment_method { get; set; }
    public int? order_status { get; set; }
    public bool? is_delete { get; set; } = false;
    public bool is_paid { get; set; } = false;
    public bool is_custom_order { get; set; } = false;
}
