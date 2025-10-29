using cake_shop_back_end.DataObjects.Requests.Configuration;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Configuration;

public interface IDeliverySlot
{
    public Task<APIResponse> GetListAsync(DeliverySlotRequest request);
    public Task<APIResponse> GetDetailAsync(int id);
    public Task<APIResponse> CreateAsync(DeliverySlotRequest request, string username);
    public Task<APIResponse> UpdateAsync(DeliverySlotRequest request, string username);
    public Task<APIResponse> DeleteAsync(DeliverySlotRequest req);
    public Task<APIResponse> ChangeStatusAsync(DeliverySlotRequest req);
    
}
