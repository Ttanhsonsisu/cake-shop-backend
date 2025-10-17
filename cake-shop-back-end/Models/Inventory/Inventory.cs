using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Inventory;

public class Inventory : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid variant_id { get; set; }

    public int quantity { get; set; }
}
