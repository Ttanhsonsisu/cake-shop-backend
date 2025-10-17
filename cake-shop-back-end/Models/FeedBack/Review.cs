using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.FeedBack;

public class Review : MasterCommonModel
{
    public Guid id { get; set; }

    public Guid? product_id { get; set; }

    public Guid? variant_id { get; set; }

    public Guid user_id { get; set; }

    public int rating { get; set; }

    public string? title { get; set; }

    public string? content { get; set; }

    public string? image_url { get; set; }

    public bool? is_approved { get; set; }

    public int? likes { get; set; }
}
