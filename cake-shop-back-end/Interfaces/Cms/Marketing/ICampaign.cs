using cake_shop_back_end.DataObjects.Requests.Marketing;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.Marketing;

public interface ICampaign
{
    public Task<APIResponse> GetListAsync(CampaignRequest request);
    public Task<APIResponse> GetDetailAsync(Guid id);
    public Task<APIResponse> CreateAsync(CampaignRequest request, string username);
    public Task<APIResponse> UpdateAsync(CampaignRequest request, string username);
    public Task<APIResponse> DeleteAsync(CampaignRequest request, string username);
    public Task<APIResponse> ChangeStatusAsync(CampaignRequest request, string username);
}
