namespace cake_shop_back_end.DataObjects.Requests.Common;

public class DeleteRequest
{
    public int? Id { get; set; }
    public int? ReferenceId { get; set; }
    public int? StatusId { get; set; }
    public string? ReasonDenine { get; set; }
    public string? RransCode { get; set; }
    public List<int>? Ids { get; set; }
}
