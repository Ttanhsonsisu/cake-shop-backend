using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Interfaces.Cms.Product;
using cake_shop_back_end.Models.Order;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Product;

public class CustomOrderOptionDataAccess(AppDbContext _context) : ICustomOrderOption
{
    public async Task<APIResponse> CreateAsync(CustomOrderOptionRequest request, string username)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return new APIResponse("NAME_REQUIRED");
        // check exist
        var dataExist = await _context.CategoryCustomOrders
            .AnyAsync(x => x.id == request.CategoryId);

        if (!dataExist)
        {
            return new APIResponse("CATEGORY_NOT_FOUND");
        }

        var entity = new CustomOrderOption
        {
            img_url = request.ImgUrl,
            category_id = request.CategoryId,
            price_modify = request.PriceModify,
            name = request.Name,
            is_active = request.IsActive ?? true,
            description = request.Description,
            orders = request.Orders,
            user_created = username,
            date_created = DateTime.UtcNow
        };

        try
        {
            await _context.Set<CustomOrderOption>().AddAsync(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return new APIResponse(200);
        }
        catch (Exception)
        {
            return new APIResponse("INTERNAL_SERVER_ERROR");
        }
    }

    public async Task<APIResponse> DeleteAsync(int id, string username)
    {
        if (id < 0) return new APIResponse("ERROR_INVALID_REQUEST");

        var data = await _context.Set<CustomOrderOption>().FindAsync(id);
        if (data == null) return new APIResponse("NOT_FOUND");

        try
        {
            _context.Set<CustomOrderOption>().Remove(data);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return new APIResponse(200);
        }
        catch (Exception)
        {
            return new APIResponse("INTERNAL_SERVER_ERROR");
        }
    }

    public async Task<APIResponse> GetDetailAsync(int id)
    {
        if (id < 0) return new APIResponse("ERROR_INVALID_REQUEST");

        var item = await _context.Set<CustomOrderOption>()
            .Where(x => x.id == id)
            .Select(x => new
            {
                x.id,
                x.img_url,
                x.category_id,
                x.price_modify,
                x.name,
                x.is_active,
                x.description,
                x.orders,
                x.user_created,
                x.date_created
            })
            .FirstOrDefaultAsync();

        if (item == null) return new APIResponse("NOT_FOUND");
        return new APIResponse(item);
    }

    public async Task<APIResponse> GetListAsync(CustomOrderOptionRequest request)
    {
        if (request.PageSize < 1) request.PageSize = 20;
        if (request.PageNo < 1) request.PageNo = 1;

        var query = _context.Set<CustomOrderOption>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var pattern = $"%{request.Name}%";
            query = query.Where(x => EF.Functions.Like(x.name, pattern));
        }

        if (request.CategoryId != null)
        {
            query = query.Where(x => x.category_id == request.CategoryId.Value);
        }

        if (request.IsActive != null)
        {
            query = query.Where(x => x.is_active == request.IsActive.Value);
        }

        var count = await query.CountAsync();
        var totalPage = count > 0 ? (int)Math.Ceiling(count / (double)request.PageSize) : 0;
        var skip = (request.PageNo - 1) * request.PageSize;

        var data = await query
            .OrderByDescending(x => x.orders)
            .ThenByDescending(x => x.date_created)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(x => new
            {
                x.id,
                x.img_url,
                x.category_id,
                x.price_modify,
                x.name,
                x.is_active,
                x.orders,
                x.date_created
            })
            .ToListAsync();

        var result = new DataListResponse
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalPage = totalPage,
            Data = data
        };

        return new APIResponse(result);
    }

    public async Task<APIResponse> UpdateAsync(CustomOrderOptionRequest request, string username)
    {
        if (request.Id == null || request.Id <= 0) return new APIResponse("ERROR_INVALID_REQUEST");

        var data = await _context.Set<CustomOrderOption>().FindAsync(request.Id.Value);
        if (data == null) return new APIResponse("NOT_FOUND");
        var dataExist = await _context.CategoryCustomOrders
            .AnyAsync(x => x.id == request.CategoryId);
        if (request.CategoryId == null && !dataExist)
        {
            return new APIResponse("CATEGORY_NOT_FOUND");
        }

        data.img_url = request.ImgUrl ?? data.img_url;
        data.category_id = request.CategoryId ?? data.category_id;
        data.price_modify = request.PriceModify ?? data.price_modify;
        data.name = request.Name ?? data.name;
        if (request.IsActive != null) data.is_active = request.IsActive;
        data.description = request.Description ?? data.description;
        data.orders = request.Orders ?? data.orders;

        try
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return new APIResponse(200);
        }
        catch (Exception)
        {
            return new APIResponse("INTERNAL_SERVER_ERROR");
        }
    }
}
