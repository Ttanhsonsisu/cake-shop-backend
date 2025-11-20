using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Product;

public class CustomOrderOptionRequest : PaggingRequest
{
    public int? Id { get; set; }
    public string? ImgUrl { get; set; }
    public int? CategoryId { get; set; }
    public decimal? PriceModify { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public string? Description { get; set; }
    public int? Orders { get; set; }
}
