namespace cake_shop_back_end.Models.Discount;

public class DiscountCodeTarget
{
    public Guid id { get; set; } = new Guid();
    public Guid campaign_id { get; set; }
    public Guid? product_id { get; set; }
    public Guid? variant_id { get; set; }
}
