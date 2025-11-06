using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Product;

public interface IAttributeValue
{
    public Task<APIResponse> GetListAsync(AttributeValueRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(AttributeValueRequest request, string username);
    public Task<APIResponse> UpdateAsync(AttributeValueRequest request, string username);
    public Task<APIResponse> DeleteAsync(AttributeValueRequest req, string username);
    public Task<APIResponse> ChangeStatusAsync(AttributeValueRequest req, string username);
    public Task<APIResponse> GetListInAttribute(AttributeValueRequest request);
}
