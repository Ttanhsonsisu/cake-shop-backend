using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Partner;

public class PartnerDelivery : MasterCommonModel
{
    public int id { get; set; }
    public string name { get; set; }
    public string? api_endpoint { get; set; }
    public string? api_key { get; set; }
    public string? contact_phone { get; set; }
    public string? contact_email { get; set; }
    public decimal? base_delivery_fee { get; set; }
    public int? status { get; set; }
    public bool is_active { get; set; } = false;
}
