using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Marketing;

public class DiscountCodeRequest : PaggingRequest
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; } = true;
    public string? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal? MaxDiscount { get; set; }
    public int? Status { get; set; }

    // Danh sách sản phẩm/variant áp dụng
    public List<DiscountCodeTargetDto>? Targets { get; set; }
}

public class DiscountCodeTargetDto
{
    public Guid? Id { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? CampaignID { get; set; } 
    public Guid? VariantId { get; set; }
}
