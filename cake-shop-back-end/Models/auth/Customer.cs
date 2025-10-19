using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.auth;

public class Customer : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public Guid user_id { get; set; }
    public string? name { get; set; }
    public string? email { get; set; }
    public string? phone { get; set; }
    public DateTime? birth_date { get; set; }
    public int? gender { get; set; }
    public int? country_id { get; set; }
    public string? address { get; set; }
    public int? states_id { get; set; }
    public string? zip_code { get; set; }
    public int? status { get; set; }
    public string? company_name { get; set; }
}
