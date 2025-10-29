namespace cake_shop_back_end.Models.Common;

public class DeliveryArea : MasterCommonModel
{
    public int id { get; set; }
    public string area_name { get; set; }
    public decimal fee { get; set; } = 0m;
    public string estimated_delivery_time { get; set; }
    public int status { get; set; }
}
