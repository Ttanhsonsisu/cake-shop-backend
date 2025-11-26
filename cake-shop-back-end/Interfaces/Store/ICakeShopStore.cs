using cake_shop_back_end.DataObjects.Requests.Store;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Store;

public interface ICakeShopStore
{
    public Task<APIResponse> GetListProducts(CakeShopRequest request);
}
