namespace cake_shop_back_end.Models.Common;

public class OtherListType : MasterCommonModel
{
    public int id { get; set; }

    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public int status { get; set; }

    public string? description { get; set; }

    public int? orders { get; set; }
}
