namespace cake_shop_back_end.Models.Post;

public class PostTag
{
    public Guid id { get; set; }

    public string name { get; set; } = null!;

    public string? slug { get; set; }
}
