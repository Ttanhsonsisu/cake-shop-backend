using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class OrderStatusHistory : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid order_id { get; set; }

    public int? old_status { get; set; }

    public int new_status { get; set; }

    public string? changed_by { get; set; }

    public DateTime? changed_date { get; set; }

    public string? note { get; set; }
}
