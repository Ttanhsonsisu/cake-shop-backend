using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.auth;

public class Function : MasterCommonModel
{
    public Guid id { get; set; }

    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public int status { get; set; }

    public bool? is_default { get; set; }

    public bool is_visible { get; set; }

    public string? url { get; set; }

    public string? web_type { get; set; }
}
