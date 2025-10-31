using cake_shop_back_end.DataObjects.Requests.Configuration;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Product;

public interface ICategory
{
    public Task<APIResponse> GetListAsync(CategoryRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(CategoryRequest request, string username);
    public Task<APIResponse> UpdateAsync(CategoryRequest request, string username);
    public Task<APIResponse> DeleteAsync(CategoryRequest req, string username);
    public Task<APIResponse> ChangeStatusAsync(CategoryRequest req, string username, string type);
}
