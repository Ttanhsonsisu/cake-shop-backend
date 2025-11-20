using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Marketing;

public class CampaignRequest : PaggingRequest
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; } = true;
    public string? DiscountType { get; set; } 
    public decimal? DiscountValue { get; set; }
    public decimal? MaxDiscount { get; set; }
    public List<DiscountTargetDto>? Targets { get; set; }
    public int? Status { get; set; }

}

public class DiscountTargetDto
{
    public Guid? ProductId { get; set; }
    public Guid? VariantId { get; set; }
}
