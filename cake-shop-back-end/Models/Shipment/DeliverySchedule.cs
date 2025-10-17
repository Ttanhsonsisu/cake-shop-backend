using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Shipment;

public class DeliverySchedule : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid order_id { get; set; }

    public DateTime delivery_date { get; set; }

    public TimeSpan? delivery_time_from { get; set; }

    public TimeSpan? delivery_time_to { get; set; }

    public string? shipper_name { get; set; }

    public string? shipper_phone { get; set; }

    public int? status { get; set; }

    public string? note { get; set; }
}
