using cake_shop_back_end.DataObjects.Requests.Marketing;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Marketing;

public interface IDiscountCode
{
    public Task<APIResponse> GetListAsync(DiscountCodeRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(DiscountCodeRequest request, string username);
    public Task<APIResponse> UpdateAsync(DiscountCodeRequest request, string username);
    public Task<APIResponse> DeleteAsync(DiscountCodeRequest request, string username);
    public Task<APIResponse> ChangeStatusAsync(DiscountCodeRequest request, string username);
}
