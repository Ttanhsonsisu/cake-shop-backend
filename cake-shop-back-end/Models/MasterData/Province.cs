using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.MasterData;

public class Province : MasterCommonModel
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public int? orders { get; set; }

    public string? code { get; set; }

    public int? parent_id { get; set; }

    public int? type { get; set; }

    public string? description { get; set; }
}
