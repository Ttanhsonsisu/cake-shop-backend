using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.MasterData;

public class ProvinceRequest : PaggingRequest
{
    public int? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int? Types { get; set; }
    public int? ParentId { get; set; }
    public int? Orders { get; set; }
}
