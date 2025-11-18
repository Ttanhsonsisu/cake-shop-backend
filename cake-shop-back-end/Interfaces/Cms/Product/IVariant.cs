using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Product;

public interface IVariant
{
    public Task<APIResponse> GetListAsync(VariantRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateDraftAsync(VariantRequest request, string username);
    public Task<APIResponse> UpdateAsync(VariantRequest request, string username);
    public Task<APIResponse> DeleteAsync(VariantRequest request, string username);
    public Task<APIResponse> AddImageAsync(ProductImageRequest request, string username);
    public Task<APIResponse> RemoveImageAsync(ProductImageRequest request, string username);
}
