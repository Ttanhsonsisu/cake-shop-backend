using cake_shop_back_end.DataObjects.Requests.Configuration;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Configuration;

public interface IDeliveryArea
{
    public Task<APIResponse> GetListAsync(DeliveryAreaRequest request);
    public Task<APIResponse> GetDetailAsync(int id);
    public Task<APIResponse> CreateAsync(DeliveryAreaRequest request, string username);
    public Task<APIResponse> UpdateAsync(DeliveryAreaRequest request, string username);
    public Task<APIResponse> DeleteAsync(DeliveryAreaRequest req, string username);
    public Task<APIResponse> ChangeStatusAsync(DeliveryAreaRequest req, string username);
}
