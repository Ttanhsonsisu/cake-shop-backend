namespace cake_shop_back_end.Models.auth;

public class UserGroupPermission
{
    public Guid id { get; set; }
    public Guid user_group_id { get; set; }
    public Guid action_id { get; set; }
}
