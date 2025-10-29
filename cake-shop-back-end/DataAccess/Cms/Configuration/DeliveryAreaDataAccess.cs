using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Configuration;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Configuration;
using cake_shop_back_end.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Configuration;

public class DeliveryAreaDataAccess(AppDbContext _context) : IDeliveryArea
{
    public async Task<APIResponse> ChangeStatusAsync(DeliveryAreaRequest req, string username)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_IS_NULL");
        }
        if (req.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        // Giả sử DbSet của bạn tên là DeliveryAreas
        var data = await _context.DeliveryAreas.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_AREA_NOT_EXISTS");
        }

        try
        {
            data.status = (int)req.Status;
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

    public async Task<APIResponse> CreateAsync(DeliveryAreaRequest request, string username)
    {
        // check data request
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }

        if (string.IsNullOrWhiteSpace(request.AreaName))
        {
            return new APIResponse("ERROR_AREANAME_REQUIRED");
        }

        if (string.IsNullOrWhiteSpace(request.EstimatedDeliveryTime))
        {
            return new APIResponse("ERROR_ESTIMATEDTIME_REQUIRED");
        }

        if (request.Fee == null || request.Fee < 0)
        {
            return new APIResponse("ERROR_FEE_INVALID");
        }

        try
        {
            var data = new DeliveryArea();
            data.area_name = request.AreaName;
            data.fee = request.Fee.Value;
            data.estimated_delivery_time = request.EstimatedDeliveryTime;
            data.status = request.Status ?? 1; 

            data.date_updated = DateTime.Now;
            data.date_created = DateTime.Now;
            data.user_created = username;
            data.user_updated = username;

            await _context.DeliveryAreas.AddAsync(data);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR " + ex.Message.ToString().ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(DeliveryAreaRequest req, string username)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }

        if (req.Id == null)
        {
            return new APIResponse("ERROR_MISSING_ID");
        }

        // CHECK EXISTS 
        var data = await _context.DeliveryAreas.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            _context.DeliveryAreas.Remove(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Giữ nguyên lỗi typo "ERORR" từ code gốc của bạn
            return new APIResponse("ERORR " + ex.Message.ToString().ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(int id)
    {
        if (id < 0)
        {
            return new APIResponse("ID_NOT_INVALID");
        }

        var data = await _context.DeliveryAreas.FindAsync(id);

        if (data == null)
        {
            return new APIResponse("ERROR_NOT_EXISTS");
        }

        var response = new
        {
            Id = data.id,
            AreaName = data.area_name,
            Fee = data.fee,
            EstimatedDeliveryTime = data.estimated_delivery_time,
            Status = data.status
        };

        return new APIResponse(response) { Code = "200" };
    }

    public async Task<APIResponse> GetListAsync(DeliveryAreaRequest request)
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

        var query = (from d in _context.DeliveryAreas
                     select new
                     {
                         id = d.id,
                         area_name = d.area_name,
                         fee = d.fee,
                         estimated_delivery_time = d.estimated_delivery_time,
                         status = d.status,
                     });

        // filter 
        if (request.AreaName != null && request.AreaName.Length > 0)
        {
            string keyword = request.AreaName.ToLower();

            query = query.Where(x =>
                (x.area_name != null && x.area_name.ToLower().Contains(keyword)) ||
                (x.estimated_delivery_time != null && x.estimated_delivery_time.ToLower().Contains(keyword))
            );
        }

        int countElements = await query.CountAsync();

        int totalPage = countElements > 0
            ? (int)Math.Ceiling(countElements / (double)request.PageSize)
            : 0;

        var data = await query
            //.OrderBy(x => x.id) 
            .Skip(skipElement)
            .Take(request.PageSize)
            .ToListAsync();

        var dataResult = new DataListResponse
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalPage = totalPage,
            Data = data,
            TotalElements = countElements
        };

        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> UpdateAsync(DeliveryAreaRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        if (string.IsNullOrEmpty(request.AreaName))
        {
            return new APIResponse("ERROR_AREANAME_MISSING");
        }
        if (string.IsNullOrWhiteSpace(request.EstimatedDeliveryTime))
        {
            return new APIResponse("ERROR_ESTIMATEDTIME_MISSING");
        }
        if (request.Fee == null || request.Fee < 0)
        {
            return new APIResponse("ERROR_FEE_INVALID");
        }

        var data = await _context.DeliveryAreas.FindAsync(request.Id);

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXIST");
        }

        try
        {
            data.area_name = request.AreaName;
            data.fee = request.Fee.Value;
            data.estimated_delivery_time = request.EstimatedDeliveryTime;
            data.status = (int)request.Status;

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
