using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Refund;

public class RefundRequest : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public Guid order_id { get; set; }
    public int refund_reason_id { get; set; }
    public string description { get; set; } 
    public int refund_method { get; set; }
    public int status { get; set; }
    public string? bank_name { get; set; }
    public string? account_number { get; set; }
    public string? name_bank { get; set; }
    public string? img_support { get; set; }
}
