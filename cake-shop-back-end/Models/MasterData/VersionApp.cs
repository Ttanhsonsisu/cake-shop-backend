using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.MasterData;

public class VersionApp : MasterCommonModel
{
    public int? id { get; set; }
    public string? name { get; set; }
    public string? version_name { get; set; }
    public int? build { get; set; }
    public string? platform { get; set; }
    public Boolean? is_active { get; set; }
    public Boolean? is_require_update { get; set; }
    public DateTime? apply_date { get; set; }
    public string? description { get; set; }
}
