using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Product;

public interface IAttribute
{
    public Task<APIResponse> GetListAsync(AttributeRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(AttributeRequest request, string username);
    public Task<APIResponse> UpdateAsync(AttributeRequest request, string username);
    public Task<APIResponse> DeleteAsync(AttributeRequest req, string username);
    public Task<APIResponse> ChangeStatusAsync(AttributeRequest req, string username);
    
}
