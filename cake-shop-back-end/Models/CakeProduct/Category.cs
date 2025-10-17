using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.CakeProduct;

public class Category : MasterCommonModel
{
    public int id { get; set; }

    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public int? status { get; set; }

    public bool is_popular { get; set; }

    public bool is_show { get; set; }

    public int? orders { get; set; }

    public int type { get; set; }

    public int? values { get; set; }
}
