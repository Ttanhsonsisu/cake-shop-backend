namespace cake_shop_back_end.Models.Seasonal;

public class SeasonalEventIncludeCategory
{
    public Guid id { get; set; } = new Guid();
    public Guid seasonal_event_id { get; set; }
    public Guid category_product_id { get; set; }
}
