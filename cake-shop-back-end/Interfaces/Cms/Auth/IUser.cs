using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Auth;

public interface IUser
{
    public Task<APIResponse> GetListAsync(UserRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(UserRequest request, string username);
    public Task<APIResponse> UpdateAsync(UserRequest request, string username);
    public Task<APIResponse> DeleteAsync(UserRequest req);
    public Task<APIResponse> ChangeStatusAsync(UserRequest req);
    public Task<APIResponse> CreateNomalUser(UserRequest req);
    //public Task<APIResponse> UpdateNomalInfo(UserRequest req);
    //public Task<APIResponse> UpdateBillngAddress(UserRequest req);

}
