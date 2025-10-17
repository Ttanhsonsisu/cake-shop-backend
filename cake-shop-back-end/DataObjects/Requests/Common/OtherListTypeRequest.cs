namespace cake_shop_back_end.DataObjects.Requests.Common;

public class OtherListTypeRequest : PaggingRequest
{
    public int? Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? Status { get; set; }

    public string? Description { get; set; }

    public int? Orders { get; set; }
}
