using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Post;

public class Post : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public string title { get; set; }
    public string? slug { get; set; }
    public string? summary { get; set; }
    public string? thumbnail { get; set; }
    public string? content { get; set; }
    public Guid? category_id { get; set; }
    public Guid? author_id { get; set; }
    public DateTime? published_date { get; set; }
    public bool is_featured { get; set; } = false;
    public int? type { get; set; }
    public int status { get; set; } = 0;
}
