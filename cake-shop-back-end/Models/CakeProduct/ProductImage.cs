using cake_shop_back_end.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.Models.CakeProduct;

[PrimaryKey(nameof(image_id))]
public class ProductImage : MasterCommonModel
{
    public long image_id { get; set; }

    public Guid? product_id { get; set; }

    public string image_url { get; set; } = null!;

    public bool? is_main { get; set; }
}
