namespace cake_shop_back_end.Models.Seasonal;

public class SeasonalEventIncludeProduct
{
    public Guid id { get; set; } = new Guid();
    public Guid seasonal_event_id { get; set; }
    public Guid product_id { get; set; }
}
