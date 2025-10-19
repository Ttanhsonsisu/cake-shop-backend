using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Auth;

public interface ICustomer
{
    public Task<APIResponse> GetListAsync(CustomerRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(CustomerRequest request);
    public Task<APIResponse> UpdateAsync(CustomerRequest request, string username);
    public Task<APIResponse> DeleteAsync(CustomerRequest req);
    public Task<APIResponse> ChangeStatusAsync(CustomerRequest req);
    public Task<APIResponse> UpdateAccountSettingAsync(Guid idUser, CustomerRequest request, string username);
    public Task<APIResponse> UpdateBillingAddressAsync(CustomerRequest request, string username);
    public Task<APIResponse> UpdateBillingAddressAsync(Guid idUser, CustomerRequest request, string username);
    public Task<APIResponse> UpdateNomalInfo(CustomerRequest request, string username);
    public Task<APIResponse> GetInforSettingAccountCustomer(Guid idUser);
    public Task<APIResponse> ChangeAvatar(CustomerRequest req, string username);
    public Task<APIResponse> GetInfoCustomerWithIdUser(Guid idUser);
    public Task<APIResponse> ChangePassword(Guid idUser, PasswordRequest req, string username);

}
