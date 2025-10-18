using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Auth;

public interface IUserGroup
{
    public Task<APIResponse> GetListAsync(UserGroupRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(UserGroupRequest request, string username);
    public Task<APIResponse> UpdateAsync(UserGroupRequest request, string username);
    public Task<APIResponse> DeleteAsync(UserGroupRequest req);
    public Task<APIResponse> GetPermissionAsync(Guid id);
    public Task<APIResponse> ChangeStatusAsync(UserGroupRequest req);
}
