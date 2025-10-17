using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Post;

public class Post : MasterCommonModel
{
    public Guid id { get; set; }

    public string title { get; set; } = null!;

    public string? slug { get; set; }

    public string? thumbnail { get; set; }

    public string? content { get; set; }

    public Guid? category_id { get; set; }

    public Guid? author_id { get; set; }

    public DateTime? published_date { get; set; }

    public int? status { get; set; }
}
