using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Configuration;

public class DeliveryAreaRequest : PaggingRequest
{
    public int? Id { get; set; }
    public string? AreaName { get; set; }
    public decimal? Fee { get; set; } = 0m;
    public string? EstimatedDeliveryTime { get; set; }
    public int? Status { get; set; }
}
