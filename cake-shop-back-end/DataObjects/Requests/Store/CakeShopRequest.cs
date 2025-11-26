using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Store;

public class CakeShopRequest : PaggingRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    // Sorting: 'featured', 'price-low', 'price-high', 'rating', 'popular'
    public string SortBy { get; set; } = "featured";

    // Text Search
    public string? SearchQuery { get; set; }

    // Filters
    public string? Category { get; set; } // Category từ URL (Main category)
    public List<string> SelectedCategories { get; set; } = []; // Category từ Checkbox sidebar

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // Attributes (Flavor, Size)
    public List<string> SelectedFlavors { get; set; } = [];
    public List<string> SelectedSizes { get; set; } = [];

}
