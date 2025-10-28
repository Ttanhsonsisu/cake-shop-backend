using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Common;

public interface IAddressInfo
{
    public Task<APIResponse> GetListAllAsync(AddressInfoRequest request);
    public Task<APIResponse> GetListAddressInCustomer(Guid idCustomer,AddressInfoRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(AddressInfoRequest request, string username);
    public Task<APIResponse> UpdateAsync(AddressInfoRequest request, string username);
    public Task<APIResponse> DeleteAsync(AddressInfoRequest req, string username);
    public Task<APIResponse> ChangeStatusAsync(AddressInfoRequest req, string username);

    
}
