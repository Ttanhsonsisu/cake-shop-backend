using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.Models.auth;

namespace cake_shop_back_end.DataObjects.Requests.Auth;

public class UserGroupRequest : PaggingRequest
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public int? Status { get; set; }
    public Guid? IssuersId { get; set; }
    public string? Description { get; set; }
    public List<UserGroupPermission>? UserGroupPermissions { get; set; }
}
