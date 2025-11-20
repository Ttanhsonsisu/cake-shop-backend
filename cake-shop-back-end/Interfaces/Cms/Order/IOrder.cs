using cake_shop_back_end.DataObjects.Requests.Order;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Order;

public interface IOrder
{
    public Task<APIResponse> CreateOrderDrafFromAdminAsync(OrderRequest request, string username);
    public Task<APIResponse> GetOrderListAsync(OrderRequest request);
    public Task<APIResponse> DetailOrderNomalAsync(Guid id);
    public Task<APIResponse> DeleteOrderAsync(OrderRequest request, string username);
    public Task<APIResponse> PublishFromAdminOrderAsync(OrderRequest request, string username);
    public Task<APIResponse> UpdateStatusOrderAsync(OrderRequest request, string username);
    public Task<APIResponse> ComfirmPaymentAsync(OrderRequest request, string username);
    public Task<APIResponse> ComfirmOrderAsync(OrderRequest request, string username);
    public Task<APIResponse> CancelOrderAsync(OrderRequest request, string username);
    public Task<APIResponse> ChangeStatusOrderAsync(OrderRequest request, string username);
}
