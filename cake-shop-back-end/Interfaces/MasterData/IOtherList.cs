using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.MasterData;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.MasterData;

public interface IOtherList
{
    public Task<APIResponse> GetList(OtherListRequest request);
    public Task<APIResponse> GetDetail(int id);
    public Task<APIResponse> Create(OtherListRequest request, string username);
    public Task<APIResponse> Update(OtherListRequest request, string username);
    public Task<APIResponse> Delete(DeleteRequest req);
}
