using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.auth;

public class Customer : MasterCommonModel
{
    public Guid id { get; set; }
    public Guid user_id { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public string? email { get; set; }
    public DateTime? birth_date { get; set; }
    public int? gender { get; set; }
    public int? country_id { get; set; }
    public int? states_id { get; set; }
    public string? xip_code { get; set; }
    public string? company_name { get; set; }
}
