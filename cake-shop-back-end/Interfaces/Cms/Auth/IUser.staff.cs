using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Auth;

public partial interface IUser
{
    public Task<APIResponse> ListStaffAsync(UserRequest request);
    
}
