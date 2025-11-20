using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.SeasonalEvent;

public class SeasonalEventRequest : PaggingRequest
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Status { get; set; }
    public string? BannerImageUrl { get; set; }
    public bool? IsShowHomepage { get; set; }
    public bool? IsIncludeCategory { get; set; }
    public bool? IsIncludeProduct { get; set; }
    public List<Guid>? ProductIds { get; set; }
    public List<Guid>? CategoryIds { get; set; }
}
