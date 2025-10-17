using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.CakeProduct;

public class VariantAttributeValue : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid attribute_id { get; set; }

    public Guid variant_id { get; set; }

    public Guid attribute_value_id { get; set; }
}
