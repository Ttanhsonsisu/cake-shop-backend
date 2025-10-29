using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Configuration;

public class PaymentMethodRequest : PaggingRequest
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? Type { get; set; }
    public string? Description { get; set; }
    public int? Status { get; set; } = 0;
    public int? StatusIntegration { get; set; }
    public string? IconImg { get; set; }
}
