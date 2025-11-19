using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Product;

public interface ICustomOrderOption
{
    Task<APIResponse> GetListAsync(CustomOrderOptionRequest request);
    Task<APIResponse> GetDetailAsync(int id);
    Task<APIResponse> CreateAsync(CustomOrderOptionRequest request, string username);
    Task<APIResponse> UpdateAsync(CustomOrderOptionRequest request, string username);
    Task<APIResponse> DeleteAsync(int id, string username);
    Task<APIResponse> ChangeStatusAsync(CustomOrderOptionRequest request, string username);
}
