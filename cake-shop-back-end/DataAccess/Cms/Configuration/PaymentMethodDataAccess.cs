using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Configuration;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Configuration;
using cake_shop_back_end.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Configuration;

public class PaymentMethodDataAccess(AppDbContext _context) : IPaymentMethod
{
    public async Task<APIResponse> ChangeStatusAsync(PaymentMethodRequest req, string username)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_IS_NULL");
        }
        if (req.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        if (req.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        var data = await _context.PaymentMethods.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_PAYMENT_NOT_EXISTS");
        }

        try
        {
            data.status = (int)req.Status;
            data.user_updated = username;
            data.date_updated = DateTime.Now;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE " + ex.Message.ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateAsync(PaymentMethodRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new APIResponse("ERROR_NAME_REQUIRED");
        }
        if (request.Type == null)
        {
            return new APIResponse("ERROR_TYPE_REQUIRED");
        }
        if (request.StatusIntegration == null)
        {
            return new APIResponse("ERROR_STATUS_INTEGRATION_REQUIRED");
        }

        try
        {
            var data = new PaymentMethod
            {
                name = request.Name,
                type = request.Type.Value,
                description = request.Description,
                status = request.Status ?? 0,
                status_integration = request.StatusIntegration.Value,
                icon_img = request.IconImg,

                date_created = DateTime.Now,
                date_updated = DateTime.Now,
                user_created = username,
                user_updated = username
            };

            await _context.PaymentMethods.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR " + ex.Message.ToString().ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(PaymentMethodRequest req, string username)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }

        if (req.Id == null)
        {
            return new APIResponse("ERROR_MISSING_ID");
        }

        var data = await _context.PaymentMethods.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            _context.PaymentMethods.Remove(data);
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

        var data = await _context.PaymentMethods.FindAsync(id);

        if (data == null)
        {
            return new APIResponse("ERROR_NOT_EXISTS");
        }

        var response = new
        {
            data.id,
            data.name,
            data.type,
            data.description,
            data.status,
            data.status_integration,
            data.icon_img
        };

        return new APIResponse(response) { Code = "200" };
    }

    public async Task<APIResponse> GetListAsync(PaymentMethodRequest request)
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

        var query = (from d in _context.PaymentMethods
                     select new
                     {
                         d.id,
                         d.name,
                         d.type,
                         d.description,
                         d.status,
                         d.status_integration,
                         d.icon_img
                     });


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
            .OrderByDescending(x => x.id)
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

    public async Task<APIResponse> UpdateAsync(PaymentMethodRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        if (string.IsNullOrEmpty(request.Name))
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }
        if (request.Type == null)
        {
            return new APIResponse("ERROR_TYPE_REQUIRED");
        }
        if (request.StatusIntegration == null)
        {
            return new APIResponse("ERROR_STATUS_INTEGRATION_REQUIRED");
        }

        var data = await _context.PaymentMethods.FindAsync(request.Id);

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXIST");
        }

        try
        {
            data.name = request.Name;
            data.type = request.Type.Value;
            data.description = request.Description;
            data.status_integration = request.StatusIntegration.Value;
            data.icon_img = request.IconImg;
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
