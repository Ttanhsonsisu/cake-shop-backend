using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Configuration;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Configuration;
using cake_shop_back_end.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Configuration;

public class DeliverySlotDataAccess(AppDbContext _context) : IDeliverySlot
{
    public async Task<APIResponse> ChangeStatusAsync(DeliverySlotRequest req)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_IS_NULL");
        }
        if (req.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        // check exists
        var data = await _context.DeliveryTimeSlots.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_DELIVERY_NOT_EXISTS");
        }

        try
        {
            data.status = (int)req.Status;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE " + ex.Message.ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateAsync(DeliverySlotRequest request, string username)
    {
        // check data request
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new APIResponse("ERROR_NAME_REQUIRED");
        }

        if (request.EndTime == default(TimeSpan))
        {
            return new APIResponse("ERROR_ENDTIME_REQUIRED");
        }

        if (request.StartTime == default(TimeSpan))
        {
            return new APIResponse("ERROR_ENDTIME_REQUIRED");
        }
        // check 
        if (request.StartTime.HasValue && request.StartTime.Value >= request.EndTime)
        {
            return new APIResponse("ERROR_STARTTIME_INVALID");
        }

        try
        {
            var data = new DeliveryTimeSlot();
            data.name = request.Name;
            data.start_time = request.StartTime.Value;
            data.end_time = request.EndTime.Value;
            data.status = request.Status;
            data.type = request.Type;
            data.description = request.Description;
            data.orders = request.Orders;

            data.date_updated = DateTime.Now;
            data.date_created = DateTime.Now;
            data.user_created = username;
            data.user_updated = username;

            await _context.DeliveryTimeSlots.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR " + ex.Message.ToString().ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(DeliverySlotRequest req)
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
        var data = await _context.DeliveryTimeSlots.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            _context.DeliveryTimeSlots.Remove(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {

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

        var data = await _context.DeliveryTimeSlots.FindAsync(id);

        if (data == null)
        {
            return new APIResponse("ERROR_NOT_EXISTS");
        }

        var response = new
        {
            Id = data.id,
            Name = data.name,
            StartTime = data.start_time,
            EndTime = data.end_time,
            Status = data.status,
            Description = data.description
        };

        return new APIResponse(response) { Code = "200" };

    }

    public async Task<APIResponse> GetListAsync(DeliverySlotRequest request)
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

        var query = (from d in _context.DeliveryTimeSlots
                     select new
                     {
                         id = d.id,
                         name = d.name,
                         status = d.status,
                         description = d.description,
                         start_time = d.start_time,
                         end_time = d.end_time,
                     });

        // fillter 
        if (request.Name != null && request.Name.Length > 0)
        {
            string keyword = request.Name.ToLower();

            query = query.Where(x =>
            (x.name != null && x.name.ToLower().Contains(keyword)) ||
            (x.description != null && x.description.ToLower().Contains(keyword))
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
            TotalElements = countElements,
        };

        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> UpdateAsync(DeliverySlotRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }
        if (string.IsNullOrEmpty(request.Name))
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }
        if (request.StartTime == null)
        {
            return new APIResponse("ERROR_START_TIME_MISSING");
        }
        if (request.EndTime == null)
        {
            return new APIResponse("ERROR_END_TIME_MISSING");
        }

        var data = await _context.DeliveryTimeSlots.FindAsync(request.Id);
       
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXIST");
        }

        try
        {
            data.name = request.Name;
            data.start_time = (TimeSpan)request.StartTime;
            data.end_time = (TimeSpan)request.EndTime;
            data.user_updated = username;
            data.date_updated = DateTime.Now;
            data.status = (int)request.Status;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_" + ex.Message.ToUpper());
        }

        return new APIResponse(200);
    }
}
