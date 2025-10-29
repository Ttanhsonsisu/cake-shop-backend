namespace cake_shop_back_end.Models.Common;

public class DeliveryTimeSlot : MasterCommonModel
{
    public int id { get; set; } 
    public string name { get; set; }
    public string? description { get; set; }
    public int? status { get; set; }
    public int? orders { get; set; }
    public int type { get; set; }
    public TimeSpan start_time { get; set; }
    public TimeSpan end_time { get; set; }
}
