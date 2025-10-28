using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Invoice;

public class Invoice : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public string invoice_no { get; set; }
    public Guid order_id { get; set; }
    public int status { get; set; }
}
