using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Inventory;

public class InventoryIngredient : MasterCommonModel
{
    public int id { get; set; }
    public string? name { get; set; }
    public decimal current_stock { get; set; }
    public string unit { get; set; }
    public int? status { get; set; }
    public decimal? minimum_threshold { get; set; }
    public string? supplier { get; set; }
    public string? note { get; set; }
    public bool? is_tracking { get; set; } = false;
}
