using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Product;

public class CategoryRequest : PaggingRequest
{
    public Guid? Id { get; set; }
    public string? Code { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? Description { get; set; }
    public int? Status { get; set; }
    public bool? IsPopular { get; set; }
    public bool? IsShow { get; set; }
    public int? Orders { get; set; }
    public int? Type { get; set; }
    public int? Values { get; set; }
    public string? TypeStatusChange { get; set; }
    public string? ImageUrl { get; set; }
}
    