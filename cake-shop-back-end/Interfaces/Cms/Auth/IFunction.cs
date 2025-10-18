using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Auth;

public interface IFunction
{
    public Task<APIResponse> GetListAsync(FunctionRequest request);
    public Task<APIResponse> GetDetaiAsync(Guid id);
    public Task<APIResponse> CreateAsync(FunctionRequest request, string username);
    public Task<APIResponse> UpdateAsync(FunctionRequest request, string username);
    public Task<APIResponse> DeleteAsync(FunctionRequest req);
    public Task<APIResponse> GetListFunctionPermissionAsync();
    public Task<APIResponse> ChangeStatusAsync(FunctionRequest req);
    public Task<APIResponse> GetFunctionTreeAsync();
}
