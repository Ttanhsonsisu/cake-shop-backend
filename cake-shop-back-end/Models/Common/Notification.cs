namespace cake_shop_back_end.Models.Common;

public class Notification : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public string? title { get; set; }
    public int Type { get; set; }
    public int? audience { get; set; }
    public int category { get; set; }
    public int recipient_type { get; set; }
    public string? redirect_link { get; set; }
    public int status { get; set; }
}
