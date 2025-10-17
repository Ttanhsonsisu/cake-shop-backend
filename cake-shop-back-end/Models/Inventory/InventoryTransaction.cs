using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Inventory;

public class InventoryTransaction : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid variant_id { get; set; }

    public string transaction_type { get; set; } = null!;

    public int quantity { get; set; }

    public string? note { get; set; }

    public DateTime? transaction_date { get; set; }
}
