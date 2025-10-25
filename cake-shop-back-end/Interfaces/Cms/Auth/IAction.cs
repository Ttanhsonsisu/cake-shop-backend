using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Auth;

public interface IAction
{
    public Task<APIResponse> GetListAsync(ActionRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(ActionRequest request, string username);
    public Task<APIResponse> UpdateAsync(ActionRequest request, string username);
    public Task<APIResponse> DeleteAsync(ActionRequest req);
    public Task<APIResponse> ChangeStatusAsync(ActionRequest req);
}
