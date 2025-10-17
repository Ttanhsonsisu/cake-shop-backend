using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Shipment;

public class DeliverySetting : MasterCommonModel
{
    public int id { get; set; }

    public int delivery_type { get; set; }

    public decimal base_fee { get; set; }

    public decimal? extra_per_km { get; set; }

    public int? estimated_time_minutes { get; set; } // time minutes

    public string? description { get; set; }

    public bool? is_active { get; set; }
}
