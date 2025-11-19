using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Product
{
    public interface ICategoryCustomOrder
    {
        Task<APIResponse> GetListAsync(CategoryCustomOrderRequest request);
        Task<APIResponse> GetDetailAsync(int id);
        Task<APIResponse> CreateAsync(CategoryCustomOrderRequest request, string username);
        Task<APIResponse> UpdateAsync(CategoryCustomOrderRequest request, string username);
        Task<APIResponse> DeleteAsync(int id, string username);
    }
}
