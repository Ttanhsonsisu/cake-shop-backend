using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Payment;

public class Payment : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid order_id { get; set; }

    public string payment_method { get; set; } = null!;

    public string? transaction_id { get; set; }

    public decimal amount { get; set; }

    public int? status { get; set; }

    public DateTime? paid_date { get; set; }

    public string? note { get; set; }
}
