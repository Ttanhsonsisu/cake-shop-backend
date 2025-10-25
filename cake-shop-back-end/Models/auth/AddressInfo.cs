using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.auth;

public class AddressInfo : MasterCommonModel
{
    public Guid id { get; set; }
    public Guid customer_id { get; set; }
    public int country_id { get; set; }
    public int states_id { get; set; }
    public string? address { get; set; }
    public string? zip_code { get; set; }
    public string? company_name { get; set; }
    public int? status { get; set; }
    public Boolean is_default { get; set; } = false;
}