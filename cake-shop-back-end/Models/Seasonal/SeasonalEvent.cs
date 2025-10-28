using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Seasonal;

public class SeasonalEvent : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public string name { get; set; }
    public string? description { get; set; }
    public DateTime start_date { get; set; }
    public DateTime end_date { get; set; }
    public int? status { get; set; }
    public string? banner_image_url { get; set; }
    public bool is_show_homepage { get; set; } = false;
    public bool is_include_catogory { get; set; } = false;
    public bool is_include_product { get; set; } = false;
}
