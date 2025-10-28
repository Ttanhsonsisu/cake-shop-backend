using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.MasterData;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.MasterData;
using cake_shop_back_end.Models.MasterData;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.MasterData;

public class ProvinceDataAccess(AppDbContext _context) : IProvince
{
    public async Task<APIResponse> GetList(ProvinceRequest request)
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

        int skipElements = (request.PageNo - 1) * request.PageSize;
      
        var lstProvince = await _context.Provinces.ToListAsync();

        if (request.Name != null && request.Name.Length > 0)
        {
            lstProvince = lstProvince?.OrderByDescending(x => x.date_created).Where(x => x.name.Contains(request.Name)).ToList();
        }

        int countElements = lstProvince.Count();

        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        var dataList = lstProvince.Take(request.PageSize * request.PageNo).Skip(skipElements).ToList();
        var dataResult = new DataListResponse { PageNo = request.PageNo, PageSize = request.PageSize, TotalElements = countElements, TotalPage = totalPage, Data = dataList };
        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> GetDetail(int id)
    {
        var data = await _context.Provinces.Where(x => x.id == id).FirstOrDefaultAsync();
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }
        return new APIResponse(data);
    }

    public async Task<APIResponse> Create(ProvinceRequest request, string username)
    {

        if (request.Name == null)
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        try
        {
            var data = new Province();
            var oldId = (from p in _context.Provinces
                         orderby p.id descending
                         select p.id
                          ).FirstOrDefault();
            data.id = oldId == null ? 1 : (oldId + 1);
            data.name = request.Name;
            data.parent_id = request.ParentId;
            data.type = request.Types;
            data.code = request.Code;
            data.user_created = username;
            data.date_created = DateTime.Now;
            data.user_updated = username;
            data.date_updated = DateTime.Now;

            await _context.Provinces.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_ADD_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> Update(ProvinceRequest request, string username)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        var data = await _context.Provinces.Where(x => x.id == request.Id).FirstOrDefaultAsync();
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        if (request.Name == null)
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        try
        {
            data.name = request.Name;
            data.parent_id = request.ParentId;
            data.type = request.Types;
            data.code = request.Code;
            data.user_updated = username;
            data.date_updated = DateTime.Now;

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
        var data = await _context.Provinces.Where(x => x.id == req.Id).FirstOrDefaultAsync();
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            _context.Provinces.Remove(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse(400);
        }

        return new APIResponse(200);
    }
}
