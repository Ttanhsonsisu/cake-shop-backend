using cake_shop_back_end.DataObjects.Requests.SeasonalEvent;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.SeasonalEvent;

public interface ISeasonalEvent
{
    public Task<APIResponse> GetListAsync(SeasonalEventRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(SeasonalEventRequest request, string username);
    public Task<APIResponse> UpdateAsync(SeasonalEventRequest request, string username);
    public Task<APIResponse> DeleteAsync(SeasonalEventRequest request, string username);
    public Task<APIResponse> ChangeStatusAsync(SeasonalEventRequest request, string username);
}
