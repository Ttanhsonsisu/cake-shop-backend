using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Product;

public interface IProductCake
{
    public Task<APIResponse> GetListAsync(ProductCakeRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateDraftAsync(ProductCakeRequest request, string username);
    public Task<APIResponse> UpdateAsync(ProductCakeRequest request, string username);
    public Task<APIResponse> DeleteAsync(ProductCakeRequest request, string username);
    public Task<APIResponse> ChangeStatusAsync(ProductCakeRequest request, string username);
    public Task<APIResponse> PublishProductAsync(ProductCakeRequest request, string username);
}
