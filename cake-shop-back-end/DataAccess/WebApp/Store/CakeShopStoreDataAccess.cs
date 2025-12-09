using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Store;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Interfaces.Store;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.WebApp.Store;

public class CakeShopStoreDataAccess(AppDbContext _context) : ICakeShopStore
{
    public async Task<APIResponse> GetListProducts(CakeShopRequest request)
    {
        try
        {
            // 1. Khởi tạo Query từ bảng chính (ProductCake)
            // KHÔNG dùng .Include() ở đây
            var query = _context.ProductCakes
                .Where(p => p.status.Value == 1 && (p.is_visible ?? false))
                .AsNoTracking();

            // 2. --- FILTERING (Dùng ID để map thủ công) ---

            // Tìm kiếm Text
            if (!string.IsNullOrEmpty(request.SearchQuery))
            {
                var search = request.SearchQuery.ToLower();
                query = query.Where(p =>
                    p.name.ToLower().Contains(search) ||
                    (p.description != null && p.description.ToLower().Contains(search))
                );
            }

            // Filter Category (Dùng bảng trung gian ProductCakeCategory map qua ID)
            // Logic: Tìm những Product có ID nằm trong danh sách ProductCakeCategory thỏa mãn điều kiện
            if (!string.IsNullOrEmpty(request.Category) && request.Category != "all")
            {
                // Lấy ID category dựa trên code (slug) từ request
                var catId = await _context.Categories
                    .Where(c => c.code == request.Category)
                    .Select(c => c.id)
                    .FirstOrDefaultAsync();

                if (catId != Guid.Empty)
                {
                    query = query.Where(p => _context.ProductCakeCategories
                        .Any(pc => pc.productCake_id == p.id && pc.category_id == catId));
                }
            }

            // Filter Selected Categories (Checkbox sidebar)
            if (request.SelectedCategories != null && request.SelectedCategories.Count > 0)
            {
                // Lấy list ID của các category được chọn
                var selectedCatIds = await _context.Categories
                    .Where(c => request.SelectedCategories.Contains(c.name))
                    .Select(c => c.id)
                    .ToListAsync();

                if (selectedCatIds.Any())
                {
                    query = query.Where(p => _context.ProductCakeCategories
                        .Any(pc => pc.productCake_id == p.id && selectedCatIds.Contains(pc.category_id)));
                }
            }

            // Filter Price
            if (request.MinPrice.HasValue)
                query = query.Where(p => p.base_price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.base_price <= request.MaxPrice.Value);

            // Filter Flavors (Truy vấn bảng VariantAttributeValue -> AttributeValue)
            if (request.SelectedFlavors != null && request.SelectedFlavors.Count > 0)
            {
                // Tìm productID có variant chứa AttributeValue là Flavor được chọn
                query = query.Where(p => _context.VariantAttributeValues
                    .Any(vav =>
                        // Join ngược lại Variant để lấy product_id
                        _context.Variants.Any(v => v.id == vav.variant_id && v.product_id == p.id) &&
                        // Join tới AttributeValue để check value
                        _context.AttributeValues.Any(av => av.id == vav.attribute_value_id && request.SelectedFlavors.Contains(av.value))
                    ));
            }

            // Filter Sizes (Tương tự Flavor)
            if (request.SelectedSizes != null && request.SelectedSizes.Count > 0)
            {
                query = query.Where(p => _context.VariantAttributeValues
                   .Any(vav =>
                       _context.Variants.Any(v => v.id == vav.variant_id && v.product_id == p.id) &&
                       _context.AttributeValues.Any(av => av.id == vav.attribute_value_id && request.SelectedSizes.Contains(av.value))
                   ));
            }

            // 3. --- SORTING ---
            query = request.SortBy switch
            {
                "price-low" => query.OrderBy(p => p.base_price),
                "price-high" => query.OrderByDescending(p => p.base_price),
                "rating" => query.OrderByDescending(p => p.date_created), // Tạm thời sort theo ngày vì rating query phức tạp
                _ => query.OrderByDescending(p => p.date_created)
            };

            // 4. --- PAGINATION ---
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // 5. --- PROJECTION (Select dữ liệu trả về) ---
            // Đây là phần quan trọng nhất: Dùng Sub-Query thay vì Navigation Property
            var products = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new 
                {
                    Id = p.id,
                    Name = p.name,
                    Description = p.description ?? "",
                    Price = p.base_price ?? 0,

                    // [SUB-QUERY] Lấy ảnh: Query trực tiếp bảng ProductImages với điều kiện product_id
                    Image = _context.ProductImages
                        .Where(img => img.product_id.Value == p.id && (img.is_main ?? false))
                        .Select(img => img.image_url)
                        .FirstOrDefault()
                        ?? _context.ProductImages
                            .Where(img => img.product_id == p.id)
                            .Select(img => img.image_url)
                            .FirstOrDefault() ?? "",

                    // [SUB-QUERY] Lấy Category Name
                    Category = _context.ProductCakeCategories
                        .Where(pc => pc.productCake_id == p.id)
                        .Join(_context.Categories,
                              pc => pc.category_id,
                              c => c.id,
                              (pc, c) => c.name)
                        .FirstOrDefault() ?? "Other",

                    // [SUB-QUERY] Lấy Flavors
                    Flavors = (from v in _context.Variants
                               join vav in _context.VariantAttributeValues on v.id equals vav.variant_id
                               join av in _context.AttributeValues on vav.attribute_value_id equals av.id
                               join a in _context.Attributes on av.attribute_id equals a.id
                               where v.product_id == p.id && a.code == "FLAVOR"
                               select av.value).Distinct().ToList(),

                    // [SUB-QUERY] Lấy Sizes
                    Sizes = (from v in _context.Variants
                             join vav in _context.VariantAttributeValues on v.id equals vav.variant_id
                             join av in _context.AttributeValues on vav.attribute_value_id equals av.id
                             join a in _context.Attributes on av.attribute_id equals a.id
                             where v.product_id == p.id && a.code == "SIZE"
                             select av.value).Distinct().ToList(),

                    // [SUB-QUERY] Tính Rating
                    Rating = _context.Reviews
                        .Where(r => r.product_id == p.id && r.is_approved == true)
                        .Average(r => (double?)r.rating) ?? 0,

                    Reviews = _context.Reviews
                        .Count(r => r.product_id == p.id && r.is_approved == true)
                })
                .ToListAsync();

            // 6. --- METADATA (Dữ liệu Sidebar) ---
            // Query các bảng độc lập, không cần quan tâm product
            var allFlavors = await _context.AttributeValues
                .Where(av => _context.Attributes.Any(a => a.id == av.attribute_id && a.code == "FLAVOR"))
                .Select(av => av.value)
                .Distinct()
                .ToListAsync();

            var allSizes = await _context.AttributeValues
                .Where(av => _context.Attributes.Any(a => a.id == av.attribute_id && a.code == "SIZE"))
                .Select(av => av.value)
                .Distinct()
                .ToListAsync();

            var allCategories = await _context.Categories
                .Where(c => c.status == 1 && c.is_show)
                .Select(c => c.name)
                .ToListAsync();

            var responseData = new 
            {
                Items = products,
                Pagination = new 
                {
                    CurrentPage = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                },
                Filters = new 
                {
                    AllFlavors = allFlavors,
                    AllSizes = allSizes,
                    AllCategories = allCategories,
                    MaxPriceAvailable = 1000
                }
            };

            return new APIResponse(200) { Data = responseData };
        }
        catch (Exception ex)
        {
            return new APIResponse(500, "Error: " + ex.Message);
        }
    }

    public async Task<APIResponse> GetProductDetail(Guid id)
    {
        try
        {
            // 1. Lấy thông tin cơ bản sản phẩm
            var product = await _context.ProductCakes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.id == id && p.status == 1 && (p.is_visible ?? false));

            if (product == null)
                return new APIResponse(404, "Sản phẩm không tồn tại hoặc đã bị ẩn.");

            // 2. Lấy danh sách ảnh (Sắp xếp ảnh chính lên đầu)
            var images = await _context.ProductImages
                .Where(img => img.product_id == id)
                .OrderByDescending(img => img.is_main)
                .Select(img => img.image_url)
                .ToListAsync();

            // 3. Lấy thông tin Variants và Attributes (Phức tạp nhất)
            // Cấu trúc mong muốn: List các Variant, mỗi Variant có giá riêng và thuộc tính (Size M, Vị Dâu)
            var variantsData = await (from v in _context.Variants
                                      join vav in _context.VariantAttributeValues on v.id equals vav.variant_id
                                      join av in _context.AttributeValues on vav.attribute_value_id equals av.id
                                      join a in _context.Attributes on av.attribute_id equals a.id
                                      where v.product_id == id
                                      select new
                                      {
                                          VariantId = v.id,
                                          Sku = v.sku,
                                          Price = v.price,
                                          AttributeCode = a.code, // SIZE, FLAVOR
                                          AttributeName = a.name,
                                          AttributeValue = av.value
                                      }).ToListAsync();

            // Group lại để ra danh sách Variants hoàn chỉnh phía Client
            // Client cần biết: Có những Size nào? Có những Vị nào? Variant nào kết hợp 2 cái đó?
            var variantsGrouped = variantsData
                .GroupBy(x => x.VariantId)
                .Select(g => new
                {
                    Id = g.Key,
                    Sku = g.First().Sku,
                    Price = g.First().Price,
                    Attributes = g.Select(x => new { x.AttributeCode, x.AttributeValue }).ToList()
                }).ToList();

            // 4. Lấy Category name
            var categoryName = await _context.ProductCakeCategories
                .Where(pc => pc.productCake_id == id)
                .Join(_context.Categories, pc => pc.category_id, c => c.id, (pc, c) => c.name)
                .FirstOrDefaultAsync();

            // 5. Build Response
            var result = new
            {
                Id = product.id,
                Name = product.name,
                Description = product.description,
                BasePrice = product.base_price,
                Category = categoryName,
                Images = images,
                Variants = variantsGrouped,
                // Tổng hợp nhanh các Option để hiển thị nút bấm (Ví dụ: List các Size có sẵn)
                AvailableOptions = new
                {
                    Sizes = variantsData.Where(x => x.AttributeCode == "SIZE").Select(x => x.AttributeValue).Distinct(),
                    Flavors = variantsData.Where(x => x.AttributeCode == "FLAVOR").Select(x => x.AttributeValue).Distinct()
                }
            };

            return new APIResponse(200) { Data = result };
        }
        catch (Exception ex)
        {
            return new APIResponse(500, "Error: " + ex.Message);
        }
    }

    public async Task<APIResponse> GetRelatedProducts(Guid currentProductId, int take)
    {
        try
        {
            // Logic: Tìm các Category của sản phẩm hiện tại -> Tìm sản phẩm khác cùng Category
            var categoryIds = await _context.ProductCakeCategories
                .Where(pc => pc.productCake_id == currentProductId)
                .Select(pc => pc.category_id)
                .ToListAsync();

            if (!categoryIds.Any()) return new APIResponse(200) { Data = new List<object>() };

            var relatedProducts = await _context.ProductCakes
                .Where(p => p.id != currentProductId && // Trừ sản phẩm đang xem
                            p.status == 1 && (p.is_visible ?? false) &&
                            _context.ProductCakeCategories.Any(pc => pc.productCake_id == p.id && categoryIds.Contains(pc.category_id)))
                .OrderBy(r => Guid.NewGuid()) // Random ngẫu nhiên
                .Take(take)
                .Select(p => new
                {
                    Id = p.id,
                    Name = p.name,
                    Price = p.base_price,
                    // Lấy ảnh đại diện nhanh
                    Image = _context.ProductImages.Where(i => i.product_id == p.id && (i.is_main ?? false))
                                    .Select(i => i.image_url).FirstOrDefault()
                                    ?? _context.ProductImages.Where(i => i.product_id == p.id).Select(i => i.image_url).FirstOrDefault()
                })
                .ToListAsync();

            return new APIResponse(200) { Data = relatedProducts };
        }
        catch (Exception ex)
        {
            return new APIResponse(500, "Error: " + ex.Message);
        }
    }

}
