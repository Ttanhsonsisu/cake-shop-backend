namespace cake_shop_back_end.Models.CakeProduct;

public class ProductCakeCategory
{
    public Guid id { get; set; }

    public Guid productCake_id { get; set; }

    public Guid category_id { get; set; }
}
