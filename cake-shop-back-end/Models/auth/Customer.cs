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
    public int? status { get; set; }
}
