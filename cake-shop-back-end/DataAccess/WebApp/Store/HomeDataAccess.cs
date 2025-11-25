using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Interfaces.Store;
using cake_shop_back_end.Models.CakeProduct;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Client;

public class HomeClientDataAccess(AppDbContext _context) : IHome
{
    // --- 1. Categories ---
    public async Task<APIResponse> GetCategories()
    {
        try
        {
            var data = await _context.Categories
                .Where(c => c.status == 1 && c.is_show)
                .OrderBy(c => c.orders)
                .Select(c => new
                {
                    Id = c.id,
                    Name = c.name,
                    Description = c.description,
                    Image = c.image_url ?? "",
                    Icon = "🎂"
                }).ToListAsync();

            return new APIResponse(data);
        }
        catch (Exception ex)
        {
            return new APIResponse(500, ex.Message);
        }
    }

    // --- 2. Featured On Sale ---
    public async Task<APIResponse> GetFeaturedOnSale()
    {
        try
        {
            // Logic: Lấy sản phẩm mới nhất có giảm giá (demo logic)
            var query = GetBaseProductQuery();

            var entities = await query
                .OrderByDescending(x => x.date_created)
                .Take(10) // Lấy 10 sp mới nhất
                .Select(p => new
                {
                    p.id,
                    p.name,
                    p.description,
                    p.base_price,
                    Image = _context.ProductImages.Where(img => (Guid)img.product_id == (Guid)p.id && (img.is_main ?? false)).Select(i => i.image_url).FirstOrDefault(),
                    Rating = _context.Reviews.Where(r => r.product_id == p.id && r.is_approved == true).Average(r => (double?)r.rating) ?? 0,
                    ReviewCount = _context.Reviews.Count(r => r.product_id == p.id && r.is_approved == true)
                }).ToListAsync();

            var result = entities.Select(p => new
            {
                Id = p.id,
                Name = p.name,
                Description = p.description,
                Price = p.base_price ?? 0,
                OriginalPrice = (p.base_price ?? 0) * 1.15m, // Demo: Giả lập giá gốc cao hơn 15%
                Image = p.Image,
                Rating = Math.Round(p.Rating, 1),
                ReviewsCount = p.ReviewCount,
                Badge = "Sale",
                Tags = new List<string> { "Deal", "Sweet" }
            }).ToList();

            return new APIResponse(result);
        }
        catch (Exception ex)
        {
            return new APIResponse(500, ex.Message);
        }
    }

    // --- 3. Seasonal Highlights ---
    public async Task<APIResponse> GetSeasonalHighlights()
    {
        try
        {
            // Tìm sự kiện đang diễn ra
            var currentEventId = await _context.SeasonalEvents
                .Where(e => e.start_date <= DateTime.Now && e.end_date >= DateTime.Now && e.status == 1)
                .Select(e => e.id)
                .FirstOrDefaultAsync();

            if (currentEventId == Guid.Empty)
            {
                return new APIResponse(200) { Data = null };
            }

            // Lấy list ID sản phẩm trong sự kiện
            var productIds = await _context.SeasonalEventIncludeProducts
                .Where(s => s.seasonal_event_id == currentEventId)
                .Select(s => s.product_id)
                .ToListAsync();

            var query = GetBaseProductQuery().Where(p => productIds.Contains(p.id));

            var entities = await query
                .Select(p => new
                {
                    p.id,
                    p.name,
                    p.description,
                    p.base_price,
                    Image = _context.ProductImages.Where(img => (Guid)img.product_id == p.id && (img.is_main ?? false)).Select(i => i.image_url).FirstOrDefault(),
                    Rating = _context.Reviews.Where(r => r.product_id == p.id).Average(r => (double?)r.rating) ?? 0
                }).ToListAsync();

            var result = entities.Select(p => new
            {
                Id = p.id,
                Name = p.name,
                Description = p.description,
                Price = p.base_price ?? 0,
                Image = p.Image,
                Rating = Math.Round(p.Rating, 1),
                Badge = "Seasonal",
                Tags = new List<string> { "Limited", "Holiday" }
            }).ToList();

            return new APIResponse(200) { Data = result };
        }
        catch (Exception ex)
        {
            return new APIResponse(500, ex.Message);
        }
    }

    // --- 4. Customer Favorites ---
    public async Task<APIResponse> GetCustomerFavorites()
    {
        try
        {
            // Logic: Lấy sản phẩm có rating cao nhất hoặc nằm trong category popular
            var query = GetBaseProductQuery();

            var entities = await query
                .OrderByDescending(p => _context.Reviews.Where(r => r.product_id == p.id).Count()) // Sắp xếp theo số lượng review
                .Take(8)
                .Select(p => new
                {
                    p.id,
                    p.name,
                    p.description,
                    p.base_price,
                    Image = _context.ProductImages.Where(img => (Guid)img.product_id == p.id && img.is_main.Value).Select(i => i.image_url).FirstOrDefault(),
                    Rating = _context.Reviews.Where(r => r.product_id == p.id).Average(r => (double?)r.rating) ?? 0,
                    ReviewCount = _context.Reviews.Count(r => r.product_id == p.id)
                }).ToListAsync();

            var result = entities.Select(p => new
            {
                Id = p.id,
                Name = p.name,
                Description = p.description,
                Price = p.base_price ?? 0,
                Image = p.Image,
                Rating = Math.Round(p.Rating, 1),
                ReviewsCount = p.ReviewCount,
                Badge = "Hot",
                Tags = new List<string> { "Bestseller", "Top Rated" }
            }).ToList();

            return new APIResponse(200) { Data = result };
        }
        catch (Exception ex)
        {
            return new APIResponse(500, ex.Message);
        }
    }

    public async Task<APIResponse> GetBanners()
    {
        try
        {
            var currentTime = DateTime.Now;

            // Logic: 
            // 1. is_active = 1 (Có hiệu lực)
            // 2. status = 1 (Đang hoạt động)
            // 3. start_date <= hiện tại (hoặc null)
            // 4. end_date >= hiện tại (hoặc null)
            // 5. Sắp xếp theo thứ tự ưu tiên (orders)

            var banners = await _context.WebSiteImgages // Lưu ý: map đúng tên bảng trong SQL của bạn là WebSiteImgage
                .Where(x => x.is_active == true
                            && x.status == 1
                            && (x.start_date == null || x.start_date <= currentTime)
                            && (x.end_date == null || x.end_date >= currentTime))
                .OrderBy(x => x.orders)
                .Select(x => new
                {
                    Id = x.id,
                    Title = x.name,            // Dùng làm Tiêu đề chính trên banner
                    Description = x.description, // Dùng làm dòng mô tả nhỏ dưới tiêu đề
                    ImageUrl = x.image_url,
                    AltText = x.name,
                    DisplayType = x.display_type
                })
                .ToListAsync();

            return new APIResponse(200) { Data = banners };
        }
        catch (Exception ex)
        {
            return new APIResponse(500, ex.Message);
        }
    }

    // --- Helper Method (Private) ---
    private IQueryable<ProductCake> GetBaseProductQuery()
    {
        // Chỉ lấy sản phẩm Active và Visible
        return _context.ProductCakes
            .Where(p => p.status.Value == 1 && p.is_visible.Value)
            .AsNoTracking();
    }
}