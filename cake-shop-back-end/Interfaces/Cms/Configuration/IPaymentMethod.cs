using cake_shop_back_end.DataObjects.Requests.Configuration;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Configuration;

public interface IPaymentMethod
{
    public Task<APIResponse> GetListAsync(PaymentMethodRequest request);
    public Task<APIResponse> GetDetailAsync(int id);
    public Task<APIResponse> CreateAsync(PaymentMethodRequest request, string username);
    public Task<APIResponse> UpdateAsync(PaymentMethodRequest request, string username);
    public Task<APIResponse> DeleteAsync(PaymentMethodRequest req, string username);
    public Task<APIResponse> ChangeStatusAsync(PaymentMethodRequest req, string username);
}
