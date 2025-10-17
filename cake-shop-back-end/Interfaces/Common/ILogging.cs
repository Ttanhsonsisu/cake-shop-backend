using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Common;

public interface ILogging
{
    public Task<APIResponse> GetListLogIn(FilterLoggingRequest req);
    public Task<APIResponse> GetListAction(FilterLoggingRequest req);
    public Task<APIResponse> GetListCallApi(FilterLoggingRequest req);
}
