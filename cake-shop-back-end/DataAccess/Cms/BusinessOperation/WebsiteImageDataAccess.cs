using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.BusinessOperation;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.BusinessOperation;
using cake_shop_back_end.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.BusinessOperation;

public class WebsiteImageDataAccess(AppDbContext _context) : IWebsiteImage
{
    public async Task<APIResponse> ChangeStatusAsync(WebsiteImageRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_IS_NULL");
        }
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        if (request.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        var data = await _context.WebSiteImgages.FindAsync(request.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            // Logic change status đơn giản (vì interface không có param 'type' như category)
            data.status = (int)request.Status;

            // Nếu có yêu cầu active/inactive đi kèm
            if (request.IsActive != null)
            {
                data.is_active = (bool)request.IsActive;
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

    public async Task<APIResponse> CreateAsync(WebsiteImageRequest request, string username)
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

        var existingCode = await _context.WebSiteImgages
            .AnyAsync(x => x.code.ToLower() == request.Code.ToLower());

        if (existingCode)
        {
            return new APIResponse("ERROR_CODE_EXISTS");
        }

        try
        {
            var data = new WebSiteImgage
            {
                code = request.Code,
                name = request.Name,
                status = request.Status ?? 1, // Mặc định
                start_date = request.StartDate,
                image_url = request.ImageUrl,
                end_date = request.EndDate,
                display_type = request.DisplayType,
                is_active = request.IsActive ?? true,
                description = request.Description,
                orders = request.Orders,

                date_created = DateTime.Now,
                date_updated = DateTime.Now,
                user_created = username,
                user_updated = username
            };

            await _context.WebSiteImgages.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR " + ex.Message.ToString().ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(WebsiteImageRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }

        if (request.Id == null)
        {
            return new APIResponse("ERROR_MISSING_ID");
        }

        var data = await _context.WebSiteImgages.FindAsync(request.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            _context.WebSiteImgages.Remove(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR " + ex.Message.ToString().ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> Detail(int id)
    {
        var data = await _context.WebSiteImgages.FindAsync(id);

        if (data == null)
        {
            return new APIResponse("ERROR_NOT_EXISTS");
        }

        var response = new
        {
            data.id,
            data.code,
            data.name,
            data.status,
            data.start_date,
            data.image_url,
            data.end_date,
            data.display_type,
            data.is_active,
            data.description,
            data.orders
        };

        return new APIResponse(response) { Code = "200" };
    }

    public async Task<APIResponse> GetAllAsync(WebsiteImageRequest request)
    {
        // Sử dụng logic phân trang giống mẫu
        if (request.PageSize < 1)
        {
            request.PageSize = Consts.PAGE_SIZE;
        }
        if (request.PageNo < 1)
        {
            request.PageNo = 1;
        }

        int skipElement = (request.PageNo - 1) * request.PageSize;

        var query = _context.WebSiteImgages.AsQueryable();

        // Filter theo Name hoặc Code
        if (!string.IsNullOrEmpty(request.Name))
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

        if (request.DisplayType != null)
        {
            query = query.Where(x => x.display_type == request.DisplayType);
        }

        // Projection (Select) để tối ưu performance giống mẫu
        var projectedQuery = query.Select(d => new
        {
            d.id,
            d.code,
            d.name,
            d.status,
            d.start_date,
            d.image_url,
            d.end_date,
            d.display_type,
            d.is_active,
            d.description,
            d.orders
        });

        int countElements = await projectedQuery.CountAsync();

        int totalPage = countElements > 0
            ? (int)Math.Ceiling(countElements / (double)request.PageSize)
            : 0;

        var data = await projectedQuery
            .OrderBy(x => x.orders)
            .ThenByDescending(x => x.id) // Thường ID mới nhất lên đầu nếu cùng Order
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

    public async Task<APIResponse> UpdateAsync(WebsiteImageRequest request, string username)
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

        var data = await _context.WebSiteImgages.FindAsync(request.Id);

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXIST");
        }

        // Kiểm tra trùng code (trừ chính nó)
        var existingCode = await _context.WebSiteImgages
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

            if (request.Status != null) data.status = request.Status.Value;

            data.start_date = request.StartDate;
            data.end_date = request.EndDate;
            data.image_url = request.ImageUrl;
            data.display_type = request.DisplayType;
            data.is_active = request.IsActive ?? data.is_active;
            data.orders = request.Orders;

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
