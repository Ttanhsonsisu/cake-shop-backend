using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.CakeProduct;

public class Attribute : MasterCommonModel
{
    public Guid id { get; set; }

    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public int? status { get; set; }

    public int? orders { get; set; }

    public string? values { get; set; }
}
