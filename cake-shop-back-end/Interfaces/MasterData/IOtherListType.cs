using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Interfaces.MasterData;

public interface IOtherListType
{
    public Task<APIResponse> GetList(OtherListTypeRequest request);
    public Task<APIResponse> GetDetail(int id);
    public Task<APIResponse> Create(OtherListTypeRequest request, string username);
    public Task<APIResponse> Update(OtherListTypeRequest request, string username);
    public Task<APIResponse> Delete(DeleteRequest req);
}
