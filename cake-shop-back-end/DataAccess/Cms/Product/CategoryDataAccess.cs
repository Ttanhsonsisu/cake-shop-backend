using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Product;
using cake_shop_back_end.Models.CakeProduct;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Product;

public class CategoryDataAccess(AppDbContext _context) : ICategory
{
    public async Task<APIResponse> ChangeStatusAsync(CategoryRequest req, string username, string type)
    {

        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_IS_NULL");
        }
        if (req.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        if (req.Status == null && req.IsPopular == null && req.IsShow == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        var data = await _context.Categories.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            switch (type)
            {
                case "POPULAR":
                    data.is_popular = (bool)req.IsPopular;
                    break;
                case "SHOW":
                    data.is_show = (bool) req.IsShow;
                    break;
                default:
                    data.status = (int)req.Status;
                    break;
            }

            data.date_updated = DateTime.Now;
            data.user_updated = username;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE " + ex.Message.ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateAsync(CategoryRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new APIResponse("ERROR_NAME_REQUIRED");
        }
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return new APIResponse("ERROR_CODE_REQUIRED");
        }
        if (request.Type == null)
        {
            return new APIResponse("ERROR_TYPE_REQUIRED");
        }

        var existingCode = await _context.Categories
            .AnyAsync(x => x.code.ToLower() == request.Code.ToLower());
        if (existingCode)
        {
            return new APIResponse("ERROR_CODE_EXISTS");
        }

        try
        {
            var data = new Category
            {
                code = request.Code,
                name = request.Name,
                description = request.Description,
                status = request.Status ?? 1,
                is_popular = request.IsPopular ?? false,
                is_show = request.IsShow ?? true,
                orders = request.Orders,
                type = request.Type.Value,
                values = request.Values,

                date_created = DateTime.Now,
                date_updated = DateTime.Now,
                user_created = username,
                user_updated = username
            };

            await _context.Categories.AddAsync(data); 
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR " + ex.Message.ToString().ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(CategoryRequest req, string username)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }

        if (req.Id == null)
        {
            return new APIResponse("ERROR_MISSING_ID");
        }

        var data = await _context.Categories.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            _context.Categories.Remove(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERORR " + ex.Message.ToString().ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        var data = await _context.Categories.FindAsync(id);

        if (data == null)
        {
            return new APIResponse("ERROR_NOT_EXISTS");
        }

        var response = new
        {
            data.id,
            data.code,
            data.name,
            data.description,
            data.status,
            data.is_popular,
            data.is_show,
            data.orders,
            data.type,
            data.values
        };

        return new APIResponse(response) { Code = "200" };
    }

    public async Task<APIResponse> GetListAsync(CategoryRequest request)
    {
        if (request.PageSize < 1)
        {
            request.PageSize = Consts.PAGE_SIZE;
        }
        if (request.PageNo < 1)
        {
            request.PageNo = 1;
        }

        int skipElement = (request.PageNo - 1) * request.PageSize;

        var query = _context.Categories.AsQueryable();

        if (request.Name != null && request.Name.Length > 0)
        {
            string keyword = request.Name.ToLower();
            query = query.Where(x =>
                (x.name != null && x.name.ToLower().Contains(keyword)) ||
                (x.code != null && x.code.ToLower().Contains(keyword)) ||
                (x.description != null && x.description.ToLower().Contains(keyword))
            );
        }

        if (request.Status != null)
        {
            query = query.Where(x => x.status == request.Status);
        }

        if (request.Type != null)
        {
            query = query.Where(x => x.type == request.Type);
        }

        var projectedQuery = query.Select(d => new
        {
            d.id,
            d.code,
            d.name,
            d.description,
            d.status,
            d.is_popular,
            d.is_show,
            d.orders,
            d.type,
            d.values
        });

        int countElements = await projectedQuery.CountAsync();

        int totalPage = countElements > 0
            ? (int)Math.Ceiling(countElements / (double)request.PageSize)
            : 0;

        var data = await projectedQuery
            .OrderBy(x => x.orders) 
            .ThenBy(x => x.name) 
            .Skip(skipElement)
            .Take(request.PageSize)
            .ToListAsync();

        var dataResult = new DataListResponse
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalPage = totalPage,
            Data = data
        };

        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> UpdateAsync(CategoryRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return new APIResponse("ERROR_CODE_MISSING");
        }
        if (request.Type == null)
        {
            return new APIResponse("ERROR_TYPE_REQUIRED");
        }

        var data = await _context.Categories.FindAsync(request.Id);

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXIST");
        }

        // Kiểm tra code đã tồn tại (nhưng không phải là của chính nó)
        var existingCode = await _context.Categories
            .AnyAsync(x => x.code.ToLower() == request.Code.ToLower() && x.id != request.Id);
        if (existingCode)
        {
            return new APIResponse("ERROR_CODE_EXISTS");
        }

        try
        {
            data.code = request.Code;
            data.name = request.Name;
            data.description = request.Description;
            data.is_popular = request.IsPopular ?? data.is_popular; 
            data.is_show = request.IsShow ?? data.is_show; 
            data.orders = request.Orders;
            data.type = request.Type.Value;
            data.values = request.Values;
            data.status = request.Status;

            data.user_updated = username;
            data.date_updated = DateTime.Now;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_" + ex.Message.ToUpper());
        }

        return new APIResponse(200);
    }
}
