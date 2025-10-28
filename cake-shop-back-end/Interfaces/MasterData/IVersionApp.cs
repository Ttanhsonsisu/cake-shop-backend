using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.MasterData;

public interface IVersionApp
{
    public Task<APIResponse> GetList(VersionAppRequest request);
    public Task<APIResponse> GetDetail(int id);
    public Task<APIResponse> Create(VersionAppRequest request, string username);
    public Task<APIResponse> Update(VersionAppRequest request, string username);
    public Task<APIResponse> Delete(DeleteRequest req);
}
