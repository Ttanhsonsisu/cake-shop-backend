using System.ComponentModel.DataAnnotations;
using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.BusinessOperation;

public class WebsiteImageRequest : PaggingRequest
{
    public int? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }

    public int? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? EndDate { get; set; }
    public int? DisplayType { get; set; }
    public bool? IsActive { get; set; } = true;
    public string? Description { get; set; }
    public int? Orders { get; set; }
}
