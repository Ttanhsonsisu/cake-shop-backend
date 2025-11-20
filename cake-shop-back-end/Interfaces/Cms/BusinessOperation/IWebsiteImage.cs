using cake_shop_back_end.DataObjects.Requests.BusinessOperation;
using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Cms.BusinessOperation;

public interface IWebsiteImage
{
    Task<APIResponse> GetAllAsync(WebsiteImageRequest request);
    Task<APIResponse> Detail(int id);
    Task<APIResponse> DeleteAsync(WebsiteImageRequest request, string usernane);
    Task<APIResponse> UpdateAsync(WebsiteImageRequest request, string usernane);
    Task<APIResponse> CreateAsync(WebsiteImageRequest request, string usernane);
    Task<APIResponse> ChangeStatusAsync(WebsiteImageRequest request, string username);

}
