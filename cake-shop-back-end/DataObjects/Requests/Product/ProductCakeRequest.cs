using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Product;

public class ProductCakeRequest : PaggingRequest
{
    public Guid Id { get; set; }

    public string? Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? BasePrice { get; set; }

    public string? Storage { get; set; }
    public int? Status { get; set; }
    public bool? IsVisible { get; set; }
    public int? Variants { get; set; }
    public List<Guid>? Categories { get; set; }
}

public class ProductImageRequest
{
    public long? Id { get; set; }
    public Guid? ProductId { get; set; }
    public string? ImageUrl { get; set; } = null!;
    public Guid? VariantId { get; set; }
}
