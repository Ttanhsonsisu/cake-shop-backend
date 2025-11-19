using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class CategoryCustomOrder : MasterCommonModel
{
    public int id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
    public int status { get; set; }
    public int? parent { get; set; }
    public string? description { get; set; }
    public int? orders { get; set; }
}
