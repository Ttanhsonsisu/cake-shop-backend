using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Interfaces.Cms.Product;
using cake_shop_back_end.Models.Order;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Product;

public class CategoryCustomOrderDataAccess(AppDbContext _context) : ICategoryCustomOrder
{
    public async Task<APIResponse> CreateAsync(CategoryCustomOrderRequest request, string username)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return new APIResponse("NAME_REQUIRED");

        var entity = new CategoryCustomOrder();
        entity.name = request.Name;
        entity.code = request.Code ?? "";
        entity.description = request.Description;
        entity.orders = request.Orders;
        entity.parent = request.Parent ?? -1;
        entity.status = request.Status ?? -1;
        entity.user_created = username;
        entity.date_created = DateTime.Now;

        try
        {
            await _context.CategoryCustomOrders.AddAsync(entity);
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
        if (id == null || id < 0) return new APIResponse("ERROR_INVALID_REQUEST");

        var data = await _context.CategoryCustomOrders.FindAsync(id);
        if (data == null) return new APIResponse("NOT_FOUND");

        try
        {
            _context.CategoryCustomOrders.Remove(data);
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
        if (id == null || id < 1) return new APIResponse("ERROR_INVALID_REQUEST");

        var item = await _context.CategoryCustomOrders
            .Where(c => c.id == id)
            .Select(c => new
            {
                c.id,
                c.name,
                c.description,
                c.status,
                c.user_created,
                c.date_created
            })
            .FirstOrDefaultAsync();

        if (item == null) return new APIResponse("NOT_FOUND");

        return new APIResponse(item);
    }

    public async Task<APIResponse> GetListAsync(CategoryCustomOrderRequest request)
    {
        if (request.PageSize < 1) request.PageSize = 20;
        if (request.PageNo < 1) request.PageNo = 1;

        var query = _context.CategoryCustomOrders.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var pattern = $"%{request.Name}%";
            query = query.Where(x => EF.Functions.Like(x.name, pattern));
        }

        if (request.Status != null)
        {
            query = query.Where(x => x.status == request.Status);
        }

        var count = await query.CountAsync();
        var totalPage = count > 0 ? (int)Math.Ceiling(count / (double)request.PageSize) : 0;
        var skip = (request.PageNo - 1) * request.PageSize;

        var data = await query
            .OrderByDescending(x => x.date_created)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(x => new
            {
                x.id,
                x.name,
                x.description,
                x.status,
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

    public async Task<APIResponse> UpdateAsync(CategoryCustomOrderRequest request, string username)
    {
        if (request.Id == null || request.Id < 0) return new APIResponse("ERROR_INVALID_REQUEST");

        var data = await _context.CategoryCustomOrders.FindAsync(request.Id);
        if (data == null) return new APIResponse("NOT_FOUND");

        data.name = request.Name ?? data.name;
        data.description = request.Description ?? data.description;
        if (request.Status != null) data.status = request.Status.Value;


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
