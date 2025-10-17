namespace cake_shop_back_end.Models.Post;

public class PostComment
{
    public Guid id { get; set; }

    public Guid post_id { get; set; }

    public Guid? user_id { get; set; }

    public string content { get; set; } = null!;

    public Guid? parent_comment_id { get; set; }

    public DateTime? created_at { get; set; }
}
