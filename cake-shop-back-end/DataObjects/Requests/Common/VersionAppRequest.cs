namespace cake_shop_back_end.DataObjects.Requests.Common;

public class VersionAppRequest : PaggingRequest
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? VersionName { get; set; }
    public int? Build { get; set; }
    public string? Platform { get; set; }
    public Boolean? IsActive { get; set; }
    public Boolean? IsRequireUpdate { get; set; }
    public string? ApplyDate { get; set; }
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
}
