using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.Models.auth;

namespace cake_shop_back_end.DataObjects.Requests.Auth;

public class UserRequest : PaggingRequest
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? Avatar { get; set; }
    public string? Description { get; set; }
    public string? Password { get; set; }
    public bool? IsSysadmin { get; set; }
    public bool? IsAdmin { get; set; }
    public bool? IsBranch { get; set; }
    public int? BranchId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public Guid? UserGroupId { get; set; }
    public string? DeviceId { get; set; }
    public int? Status { get; set; }
    public List<UserPermission>? UserPermissions { get; set; }
}
