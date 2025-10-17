using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.CakeProduct;

public class VariantImage : MasterCommonModel
{
    public long variant_image_id { get; set; }

    public long? variant_id { get; set; }

    public string image_url { get; set; } = null!;

    public bool? is_main { get; set; }
}
