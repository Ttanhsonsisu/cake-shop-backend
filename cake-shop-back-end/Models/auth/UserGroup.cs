using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.auth;

public class UserGroup : MasterCommonModel
{
    public Guid id { get; set; }

    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public int status { get; set; }

    public string? description { get; set; }
}
