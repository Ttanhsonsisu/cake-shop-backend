namespace cake_shop_back_end.DataObjects.Responses;

public class DataListResponse
{
    public int TotalElements { get; set; }
    public int TotalPage { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public object? Data { get; set; }
    public decimal? DataCount { get; set; }
    public decimal? Values { get; set; }
    public decimal? DataQuantity { get; set; }
}
