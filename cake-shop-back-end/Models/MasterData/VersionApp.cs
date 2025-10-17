using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.MasterData;

public class VersionApp : MasterCommonModel
{
    public Guid id { get; set; }

    public string name { get; set; } = null!;

    public int type { get; set; }

    public string code { get; set; } = null!;

    public int status { get; set; }

    public string? description { get; set; }
}
