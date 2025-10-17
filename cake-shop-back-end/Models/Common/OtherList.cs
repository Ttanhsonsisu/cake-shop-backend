namespace cake_shop_back_end.Models.Common;

public class OtherList : MasterCommonModel
{
    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public int? status { get; set; }

    public int? orders { get; set; }

    public int type { get; set; }

    public int id { get; set; }

    public int? values { get; set; }
}
