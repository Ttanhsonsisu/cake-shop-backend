using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.CakeProduct;

public class AttributeValue : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid? attribute_id { get; set; }

    public string value { get; set; } = null!;
}
