using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Shipment;

public class Shipment : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid order_id { get; set; }

    public string? shipping_code { get; set; }

    public string? carrier { get; set; }

    public int? shipping_status { get; set; }

    public DateTime? shipped_date { get; set; }

    public DateTime? delivered_date { get; set; }

    public string? note { get; set; }
}
