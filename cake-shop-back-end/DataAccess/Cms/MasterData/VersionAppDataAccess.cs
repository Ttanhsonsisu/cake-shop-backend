using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.MasterData;
using cake_shop_back_end.Models.MasterData;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.MasterData;

public class VersionAppDataAccess(AppDbContext _context, ICommonFunction _commonFunction) : IVersionApp
{
    public async Task<APIResponse> GetList(VersionAppRequest request)
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
        // Số lượng Skip
        int skipElements = (request.PageNo - 1) * request.PageSize;

        var lstAppVersion = (from p in _context.VersionApps
                             orderby p.date_created descending
                             select new
                             {
                                 id = p.id,
                                 Name = p.name,
                                 VersionName = p.version_name,
                                 build = p.build,
                                 platform = p.platform,
                                 apply_date = _commonFunction.ConvertDateToStringSort(p.apply_date)
                             });
        // Nếu tồn tại Where theo tên
        if (request.Name != null && request.Name.Length > 0)
        {
            lstAppVersion = lstAppVersion.Where(x => x.Name.Contains(request.Name) || x.VersionName.Contains(request.Name) || x.platform.Contains(request.Name));
        }


        // Đếm số lượng
        int countElements = lstAppVersion.Count();

        // Số lượng trang
        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        // Data Sau phân trang
        var dataList = await lstAppVersion.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();
        var dataResult = new DataListResponse { PageNo = request.PageNo, PageSize = request.PageSize, TotalElements = countElements, TotalPage = totalPage, Data = dataList };
        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> GetDetail(int id)
    {
        var action = await (from p in _context.VersionApps
                      where p.id == id
                      select new
                      {
                          id = p.id,
                          Name = p.name,
                          VersionName = p.version_name,
                          build = p.build,
                          platform = p.platform,
                          apply_date = _commonFunction.ConvertDateToStringSort(p.apply_date),
                          created_at = _commonFunction.ConvertDateToStringSort(p.date_created),
                          updated_at = _commonFunction.ConvertDateToStringSort(p.date_updated),
                          is_active = p.is_active,
                          is_require_update = p.is_require_update
                      }).FirstOrDefaultAsync();

        if (action == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }
        return new APIResponse(action);
    }

    public async Task<APIResponse> Create(VersionAppRequest request, string userName)
    {
        if (request.VersionName == null || request.VersionName == "")
        {
            return new APIResponse("ERROR_VersionName_MISSING");
        }

        if (request.Platform == null || request.Platform == "")
        {
            return new APIResponse("ERROR_PLATFORM_MISSING");
        }

        var dataCode = await _context.VersionApps.Where(x => x.version_name == request.VersionName && x.platform == request.Platform).FirstOrDefaultAsync();

        if (dataCode != null)
        {
            return new APIResponse("ERROR_VersionName_EXISTS");
        }

        if (request.Name == null || request.Name == "")
        {
            return new APIResponse("ERROR_Name_MISSING");
        }

        if (request.Build == null)
        {
            return new APIResponse("ERROR_BUILD_MISSING");
        }



        if (request.ApplyDate == null || request.ApplyDate == "")
        {
            return new APIResponse("ERROR_APPLY_DATE_MISSING");
        }

        try
        {
            var data = new VersionApp();
            data.name = request.Name;
            data.version_name = request.VersionName;
            data.build = request.Build;
            data.platform = request.Platform;
            data.is_active = request.IsActive != null ? request.IsActive : true;
            data.is_require_update = request.IsRequireUpdate != null ? request.IsRequireUpdate : false;
            data.apply_date = _commonFunction.ConvertStringSortToDate(request.ApplyDate);
            data.date_created = DateTime.Now;
            data.date_updated = DateTime.Now;

            await _context.VersionApps.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_ADD_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> Update(VersionAppRequest request, string userName)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        if (request.VersionName == null || request.VersionName == "")
        {
            return new APIResponse("ERROR_VersionName_MISSING");
        }

        if (request.Platform == null)
        {
            return new APIResponse("ERROR_PLATFORM_MISSING");
        }

        var dataCode = await _context.VersionApps.Where(x => x.version_name == request.VersionName && x.platform == request.Platform && x.id != request.Id).FirstOrDefaultAsync();

        if (dataCode != null)
        {
            return new APIResponse("ERROR_VersionName_EXISTS");
        }

        if (request.Name == null || request.Name == "")
        {
            return new APIResponse("ERROR_Name_MISSING");
        }

        if (request.Build == null)
        {
            return new APIResponse("ERROR_BUILD_MISSING");
        }


        if (request.ApplyDate == null || request.ApplyDate == "")
        {
            return new APIResponse("ERROR_APPLY_DATE_MISSING");
        }
        var data = await _context.VersionApps.Where(x => x.id == request.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_INCORRECT");
        }
        try
        {
            data.name = request.Name;
            data.version_name = request.VersionName;
            data.build = request.Build;
            data.platform = request.Platform;
            data.is_active = request.IsActive != null ? request.IsActive : true;
            data.is_require_update = request.IsRequireUpdate != null ? request.IsRequireUpdate : false;
            data.apply_date = _commonFunction.ConvertStringSortToDate(request.ApplyDate);

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
        var data = await _context.VersionApps.Where(x => x.id == req.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.VersionApps.Remove(data);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            await transaction.DisposeAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            //await transaction.DisposeAsync();

            return new APIResponse(400);
        }

        return new APIResponse(200);
    }
}
