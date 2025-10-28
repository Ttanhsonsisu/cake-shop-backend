namespace cake_shop_back_end.DataObjects.Requests.Common;

public class PaggingRequest
{

    public int PageNo { get; set; } = 0;

    public int PageSize { get; set; }
}
