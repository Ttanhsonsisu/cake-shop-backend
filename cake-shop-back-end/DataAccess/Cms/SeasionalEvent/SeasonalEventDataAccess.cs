using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.SeasonalEvent;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Interfaces.Cms.SeasonalEvent;
using cake_shop_back_end.Models.Seasonal;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.SeasionalEvent;

public class SeasonalEventDataAccess(AppDbContext _context) : ISeasonalEvent
{
    public async Task<APIResponse> GetListAsync(SeasonalEventRequest request)
    {
        if (request.PageSize < 1) request.PageSize = 10;
        if (request.PageNo < 1) request.PageNo = 1;

        var query = _context.Set<SeasonalEvent>().AsQueryable();

        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(x => x.name.ToLower().Contains(request.Name.ToLower()));
        }
        if (request.Status != null)
        {
            query = query.Where(x => x.status == request.Status);
        }
        if (request.StartDate != null)
        {
            query = query.Where(x => x.end_date >= request.StartDate);
        }
        if (request.IsShowHomepage != null)
        {
            query = query.Where(x => x.is_show_homepage == request.IsShowHomepage);
        }

        int totalRecord = await query.CountAsync();
        int totalPage = (int)Math.Ceiling(totalRecord / (double)request.PageSize);

        var data = await query
            .OrderByDescending(x => x.date_created)
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var response = new DataListResponse
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalPage = totalPage,
            Data = data
        };

        return new APIResponse(response);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        var eventData = await _context.Set<SeasonalEvent>().FindAsync(id);
        if (eventData == null)
        {
            return new APIResponse("ERROR_NOT_FOUND");
        }

        // Lấy danh sách Product ID đi kèm
        var productIds = await _context.Set<SeasonalEventIncludeProduct>()
            .Where(x => x.seasonal_event_id == id)
            .Select(x => x.product_id)
            .ToListAsync();

        // Lấy danh sách Category ID đi kèm
        var categoryIds = await _context.Set<SeasonalEventIncludeCategory>()
            .Where(x => x.seasonal_event_id == id)
            .Select(x => x.category_product_id)
            .ToListAsync();

        var result = new
        {
            Info = eventData,
            ProductIds = productIds,
            CategoryIds = categoryIds
        };

        return new APIResponse(result) { Code = "200" };
    }

    public async Task<APIResponse> CreateAsync(SeasonalEventRequest request, string username)
    {
        if (request == null) return new APIResponse("ERROR_REQUEST_NULL");
        if (string.IsNullOrEmpty(request.Name)) return new APIResponse("ERROR_NAME_REQUIRED");
        if (request.StartDate == null || request.EndDate == null) return new APIResponse("ERROR_DATE_REQUIRED");

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newId = Guid.NewGuid();

                // 1. Insert Main Event
                var entity = new SeasonalEvent
                {
                    id = newId,
                    name = request.Name,
                    description = request.Description,
                    start_date = request.StartDate.Value,
                    end_date = request.EndDate.Value,
                    status = request.Status ?? 1,
                    banner_image_url = request.BannerImageUrl,
                    is_show_homepage = request.IsShowHomepage ?? false,
                    is_include_catogory = request.IsIncludeCategory ?? false,
                    is_include_product = request.IsIncludeProduct ?? false,

                    user_created = username,
                    date_created = DateTime.Now,
                    user_updated = username,
                    date_updated = DateTime.Now
                };

                await _context.Set<SeasonalEvent>().AddAsync(entity);

                // 2. Insert Included Products
                if (request.ProductIds != null && request.ProductIds.Any())
                {
                    var products = request.ProductIds.Select(pid => new SeasonalEventIncludeProduct
                    {
                        id = Guid.NewGuid(),
                        seasonal_event_id = newId,
                        product_id = pid
                    }).ToList();
                    await _context.Set<SeasonalEventIncludeProduct>().AddRangeAsync(products);
                }

                // 3. Insert Included Categories
                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    var categories = request.CategoryIds.Select(cid => new SeasonalEventIncludeCategory
                    {
                        id = Guid.NewGuid(),
                        seasonal_event_id = newId,
                        category_product_id = cid
                    }).ToList();
                    await _context.Set<SeasonalEventIncludeCategory>().AddRangeAsync(categories);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new APIResponse(200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new APIResponse("ERROR_" + ex.Message.ToUpper());
            }
        });
    }

    public async Task<APIResponse> UpdateAsync(SeasonalEventRequest request, string username)
    {
        if (request.Id == null) return new APIResponse("ERROR_ID_MISSING");

        var entity = await _context.Set<SeasonalEvent>().FindAsync(request.Id);
        if (entity == null) return new APIResponse("ERROR_NOT_FOUND");

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update Info
                entity.name = request.Name ?? entity.name;
                entity.description = request.Description;
                if (request.StartDate.HasValue) entity.start_date = request.StartDate.Value;
                if (request.EndDate.HasValue) entity.end_date = request.EndDate.Value;
                if (request.Status.HasValue) entity.status = request.Status.Value;
                entity.banner_image_url = request.BannerImageUrl;

                if (request.IsShowHomepage.HasValue) entity.is_show_homepage = request.IsShowHomepage.Value;
                if (request.IsIncludeCategory.HasValue) entity.is_include_catogory = request.IsIncludeCategory.Value;
                if (request.IsIncludeProduct.HasValue) entity.is_include_product = request.IsIncludeProduct.Value;

                entity.user_updated = username;
                entity.date_updated = DateTime.Now;

                // Update Products: Xóa cũ -> Thêm mới
                if (request.ProductIds != null)
                {
                    var oldProducts = _context.Set<SeasonalEventIncludeProduct>().Where(x => x.seasonal_event_id == request.Id);
                    _context.Set<SeasonalEventIncludeProduct>().RemoveRange(oldProducts);

                    var newProducts = request.ProductIds.Select(pid => new SeasonalEventIncludeProduct
                    {
                        id = Guid.NewGuid(),
                        seasonal_event_id = request.Id.Value,
                        product_id = pid
                    }).ToList();
                    await _context.Set<SeasonalEventIncludeProduct>().AddRangeAsync(newProducts);
                }

                // Update Categories: Xóa cũ -> Thêm mới
                if (request.CategoryIds != null)
                {
                    var oldCategories = _context.Set<SeasonalEventIncludeCategory>().Where(x => x.seasonal_event_id == request.Id);
                    _context.Set<SeasonalEventIncludeCategory>().RemoveRange(oldCategories);

                    var newCategories = request.CategoryIds.Select(cid => new SeasonalEventIncludeCategory
                    {
                        id = Guid.NewGuid(),
                        seasonal_event_id = request.Id.Value,
                        category_product_id = cid
                    }).ToList();
                    await _context.Set<SeasonalEventIncludeCategory>().AddRangeAsync(newCategories);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new APIResponse(200, "Updated successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new APIResponse("ERROR_" + ex.Message.ToUpper());
            }
        });
    }

    public async Task<APIResponse> ChangeStatusAsync(SeasonalEventRequest request, string username)
    {
        if (request.Id == null) return new APIResponse("ERROR_INVALID_REQUEST");

        if (request.IsShowHomepage == null && request.Status == null) return new APIResponse("ERROR_MISSING_STATUS");

        var entity = await _context.Set<SeasonalEvent>().FindAsync(request.Id);
        if (entity == null) return new APIResponse("ERROR_NOT_FOUND");

        try
        {
            if (request.IsShowHomepage != null) entity.is_show_homepage = request.IsShowHomepage.Value;


            if (request.Status != null) entity.status = request.Status.Value;

            entity.user_updated = username;
            entity.date_updated = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_" + ex.Message.ToUpper());
        }
        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(SeasonalEventRequest request, string username)
    {
        if (request.Id == null) return new APIResponse("ERROR_ID_MISSING");

        var entity = await _context.Set<SeasonalEvent>().FindAsync(request.Id);
        if (entity == null) return new APIResponse("ERROR_NOT_FOUND");

        try
        {
            // Xóa dữ liệu bảng con trước
            var products = _context.Set<SeasonalEventIncludeProduct>().Where(x => x.seasonal_event_id == request.Id);
            _context.Set<SeasonalEventIncludeProduct>().RemoveRange(products);

            var categories = _context.Set<SeasonalEventIncludeCategory>().Where(x => x.seasonal_event_id == request.Id);
            _context.Set<SeasonalEventIncludeCategory>().RemoveRange(categories);

            // Xóa bảng cha
            _context.Set<SeasonalEvent>().Remove(entity);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_" + ex.Message.ToUpper());
        }
        return new APIResponse(200);
    }
}
