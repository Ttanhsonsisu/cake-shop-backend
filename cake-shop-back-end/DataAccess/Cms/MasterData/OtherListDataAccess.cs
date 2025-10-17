using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Requests.MasterData;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.MasterData;
using cake_shop_back_end.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.MasterData;

public class OtherListDataAccess(AppDbContext _context) : IOtherList
{
    public async Task<APIResponse> GetList(OtherListRequest request)
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

        var lstOtherList = (from p in _context.OtherLists
                            join f in _context.OtherListTypes on p.type equals f.id into fs
                            from f in fs.DefaultIfEmpty()
                            orderby p.date_created descending
                            select new
                            {
                                id = p.id,
                                code = p.code,
                                name = p.name,
                                description = p.description,
                                status = p.status,
                                orders = p.orders,
                                type = p.type,
                                type_name = f == null ? "" : f.name,
                                user_created = p.user_created,
                                user_updated = p.user_updated,
                                date_created = p.date_created,
                                date_updated = p.date_updated
                            });
        // Nếu tồn tại Where theo tên
        if (request.Name != null && request.Name.Length > 0)
        {
            lstOtherList = lstOtherList.Where(x => x.name.Contains(request.Name));
        }

        // Đếm số lượng
        int countElements = lstOtherList.Count();

        // Số lượng trang
        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        // Data Sau phân trang
        var dataList = await lstOtherList.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();
        var dataResult = new DataListResponse { PageNo = request.PageNo, PageSize = request.PageSize, TotalElements = countElements, TotalPage = totalPage, Data = dataList };

        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> GetDetail(int id)
    {
        var action = await (from p in _context.OtherLists
                            join f in _context.OtherListTypes on p.type equals f.id into fs
                            from f in fs.DefaultIfEmpty()
                            where p.id == id
                            select new
                            {
                                id = p.id,
                                code = p.code,
                                name = p.name,
                                description = p.description,
                                orders = p.orders,
                                status = p.status,
                                type = p.type,
                                type_name = f == null ? "" : f.name,
                                user_created = p.user_created,
                                user_updated = p.user_updated,
                                date_created = p.date_created,
                                date_updated = p.date_updated
                            }).FirstOrDefaultAsync();

        if (action == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        return new APIResponse(action);
    }

    public async Task<APIResponse> Create(OtherListRequest request, string username)
    {
        if (request.Code == null)
        {
            return new APIResponse("ERROR_CODE_MISSING");
        }

        var dataSame = await _context.OtherLists.Where(x => x.code == request.Code).FirstOrDefaultAsync();

        if (dataSame != null)
        {
            return new APIResponse("ERROR_CODE_EXIST");
        }

        if (request.Name == null)
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        if (request.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        if (request.Type == null)
        {
            return new APIResponse("ERROR_TYPE_ID_MISSING");
        }

        try
        {
            var data = new OtherList();
            var oldId = (from p in _context.OtherLists
                         orderby p.id descending
                         select p.id).FirstOrDefault();
            data.id = (oldId != null) ? (oldId + 1) : 1;
            data.code = request.Code;
            data.name = request.Name;
            data.description = request.Description;
            data.status = 1;
            data.type = (int)request.Type;
            data.orders = request.Orders;
            data.user_created = username;
            data.user_updated = username;
            data.date_created = DateTime.Now;
            data.date_updated = DateTime.Now;

            await _context.OtherLists.AddAsync(data);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_ADD_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> Update(OtherListRequest request, string username)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        if (request.Type == null)
        {
            return new APIResponse("ERROR_FUNC_ID_MISSING");
        }
        var data = await _context.OtherLists.Where(x => x.id == request.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            if (request.Code != null && request.Code.Length > 0)
            {
                data.code = request.Code;
                var dataSame = await _context.OtherLists.Where(x => x.code == request.Code && x.id != request.Id).FirstOrDefaultAsync();

                if (dataSame != null)
                {
                    return new APIResponse("ERROR_CODE_EXIST");
                }

            }

            if (request.Name != null && request.Name.Length > 0)
            {
                data.name = request.Name;
            }

            if (request.Type != null)
            {
                data.type = (int)request.Type;
            }

            data.description = request.Description;
            data.orders = request.Orders;
            if (request.Status != null)
            {
                data.status = request.Status;
            }

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
        var data = await _context.OtherLists.Where(x => x.id == req.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.OtherLists.Remove(data);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            await transaction.DisposeAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();

            return new APIResponse("ERROR_DELETE_FAIL");
        }

        return new APIResponse(200);
    }
}
