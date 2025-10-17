using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.MasterData;
using cake_shop_back_end.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.MasterData;

public class OtherListTypeDataAccess(AppDbContext _context) : IOtherListType
{
    public async Task<APIResponse> GetList(OtherListTypeRequest request)
    {
        // Default PageNo, PageSize
        if (request.PageSize < 1)
        {
            request.PageSize = Consts.PAGE_SIZE;
        }

        if (request.PageNo < 1)
        {
            request.PageNo = 1;
        }
        // element to skip
        int skipElements = (request.PageNo - 1) * request.PageSize;
        //.Take(request.PageSize).Skip(skipElements)
        var lstOtherListType = await _context.OtherListTypes.OrderByDescending(x => x.date_created).ToListAsync();

        // if Name filter exists
        if (request.Name != null && request.Name.Length > 0)
        {
            lstOtherListType = lstOtherListType.Where(x => x.name.Contains(request.Name))?.ToList();
        }

        int countElements = 0;

        if (lstOtherListType != null)
        {
            countElements = lstOtherListType.Count;
        }
        // numer of pages
        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        // data after paging
        var dataList = lstOtherListType.Take(request.PageSize * request.PageNo).Skip(skipElements).ToList();
        var dataResult = new DataListResponse { PageNo = request.PageNo, PageSize = request.PageSize, TotalElements = countElements, TotalPage = totalPage, Data = dataList };
        
        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> GetDetail(int id)
    {
        var otherListType = await _context.OtherListTypes.Where(x => x.id == id).FirstOrDefaultAsync();

        if (otherListType == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }
        return new APIResponse(otherListType);
    }

    public async Task<APIResponse> Create(OtherListTypeRequest request, string username)
    {
        if (request.Code == null)
        {
            return new APIResponse("ERROR_CODE_MISSING");
        }

        var dataSame = await _context.OtherListTypes.Where(x => x.code == request.Code).FirstOrDefaultAsync();

        if (dataSame != null)
        {
            return new APIResponse("ERROR_CODE_EXIST");
        }

        if (request.Name == null)
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        try
        {
            var data = new OtherListType();
            data.code = request.Code;
            data.name = request.Name;
            data.description = request.Description;
            data.orders = request.Orders;
            data.status = 1;
            data.user_created = username;
            data.user_updated = username;
            data.date_created = DateTime.Now;
            data.date_updated = DateTime.Now;
            await _context.OtherListTypes.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_ADD_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> Update(OtherListTypeRequest request, string username)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        var otherListType = await _context.OtherListTypes.Where(x => x.id == request.Id).FirstOrDefaultAsync();
        if (otherListType == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        if (request.Code != null && request.Code.Length > 0)
        {
            otherListType.code = request.Code;
            var dataSame = await _context.OtherListTypes.Where(x => x.code == request.Code && x.id != request.Id).FirstOrDefaultAsync();

            if (dataSame != null)
            {
                return new APIResponse("ERROR_CODE_EXIST");
            }

        }

        if (request.Name != null && request.Name.Length > 0)
        {
            otherListType.name = request.Name;
        }

        otherListType.description = request.Description;
        otherListType.orders = request.Orders;
        otherListType.status = (int)request.Status;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE_FAIL");
        }
        return new APIResponse(200);
    }

    public async Task<APIResponse> Delete(DeleteRequest req)
    {
        var otherListType = await _context.OtherListTypes.Where(x => x.id == req.Id).FirstOrDefaultAsync();
        if (otherListType == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            _context.OtherListTypes.Remove(otherListType);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse(400);
        }

        return new APIResponse(200);
    }

}
