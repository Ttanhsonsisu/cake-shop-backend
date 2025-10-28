namespace cake_shop_back_end.Models.Common;

public class PaymentMethod : MasterCommonModel
{
    public int id { get; set; }
    public string name { get; set; }
    public int type { get; set; }
    public string? description { get; set; }
    public int status { get; set; } = 0;
    public int status_integration { get; set; }
    public string? icon_img { get; set; }
}
