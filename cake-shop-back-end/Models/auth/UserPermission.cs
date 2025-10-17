namespace cake_shop_back_end.Models.auth;

public class UserPermission
{
    public Guid id { get; set; }
    public Guid user_id { get; set; }
    public Guid action_id { get; set; }
}
