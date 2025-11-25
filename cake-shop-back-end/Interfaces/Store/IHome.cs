using cake_shop_back_end.DataObjects.Responses;

namespace cake_shop_back_end.Interfaces.Store;

public interface IHome
{
    public Task<APIResponse> GetCategories();

    public Task<APIResponse> GetFeaturedOnSale();

    public Task<APIResponse> GetSeasonalHighlights();

    public Task<APIResponse> GetCustomerFavorites();
    public Task<APIResponse> GetBanners();
}
