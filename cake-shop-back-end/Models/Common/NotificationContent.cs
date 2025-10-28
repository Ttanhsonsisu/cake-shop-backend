namespace cake_shop_back_end.Models.Common;

public class NotificationContent : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public Guid notification_id { get; set; }
    public int? type { get; set; }
    public string? subject { get; set; }
    public string? body { get; set; }
}
