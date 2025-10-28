namespace cake_shop_back_end.Models.Common;

public class WebSiteImgage : MasterCommonModel
{
    public int id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
    public int status { get; set; }
    public DateTime? start_date { get; set; }
    public string? image_url { get; set; }
    public DateTime? end_date { get; set; }
    public int? display_type { get; set; }
    public bool is_active { get; set; } = true;
    public string? description { get; set; }
    public int? orders { get; set; }
}
