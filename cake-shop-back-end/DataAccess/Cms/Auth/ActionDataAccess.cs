using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Auth;
using cake_shop_back_end.Models.auth;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Auth;

public class ActionDataAccess(AppDbContext _context) : IAction
{
    public async Task<APIResponse> ChangeStatusAsync(ActionRequest req)
    {
        var data = await _context.Actions.Where(x => x.id == req.Id).FirstOrDefaultAsync();
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        if (req.Status == null)
        {
            return new APIResponse("ERROR_STATUS_ID_MISSING");
        }

        if (data.status == req.Status)
        {
            return new APIResponse("ERROR_STATUS_CANNOT_CHANGE");
        }

        try
        {
            data.status = (int)req.Status;
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            return new APIResponse(400);
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateAsync(ActionRequest request, string username)
    {
        if (request.Code == null)
        {
            return new APIResponse("ERROR_CODE_MISSING");
        }

        var dataCode = await _context.Actions.Where(x => x.code == request.Code && x.function_id == request.FunctionId).FirstOrDefaultAsync();

        if (dataCode != null)
        {
            return new APIResponse("ERROR_CODE_EXISTS");
        }

        if (request.Name == null)
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        if (request.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        if (request.FunctionId == null)
        {
            return new APIResponse("ERROR_FUNC_ID_MISSING");
        }

        if (request.Url == null)
        {
            return new APIResponse("ERROR_URL_MISSING");
        }

        try
        {
            var data = new Action1();
            data.code = request.Code;
            data.name = request.Name;
            data.description = request.Description;
            data.status = 1;
            data.function_id = (Guid)request.FunctionId;
            data.is_default = request.IsDefault ?? false;
            data.url = request.Url;
            data.user_created = username;
            data.user_updated = username;
            data.date_created = DateTime.Now;
            data.date_updated = DateTime.Now;

            await _context.Actions.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_ADD_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(ActionRequest req)
    {
        var data = await _context.Actions.Where(x => x.id == req.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Actions.Remove(data);
           await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            await transaction.DisposeAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();

            return new APIResponse(400);
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        var action = await (from p in _context.Actions
                      join f in _context.Functions on p.function_id equals f.id into fs
                      from f in fs.DefaultIfEmpty()
                      where p.id == id
                      select new
                      {
                          id = p.id,
                          code = p.code,
                          name = p.name,
                          description = p.description,
                          is_default = p.is_default,
                          status = p.status,
                          function_id = p.function_id,
                          function_name = f == null ? "" : f.name,
                          url = p.url,
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

    public async Task<APIResponse> GetListAsync(ActionRequest request)
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

        var lstAction = (from p in _context.Actions
                         join f in _context.Functions on p.function_id equals f.id into fs
                         from f in fs.DefaultIfEmpty()
                         orderby p.date_created descending
                         select new
                         {
                             id = p.id,
                             code = p.code,
                             name = p.name,
                             description = p.description,
                             is_default = p.is_default,
                             status = p.status,
                             function_id = p.function_id,
                             function_name = f == null ? "" : f.name,
                             url = p.url,
                             user_created = p.user_created,
                             user_updated = p.user_updated,
                             date_created = p.date_created,
                             date_updated = p.date_updated
                         });
     
        if (request.Name != null && request.Name.Length > 0)
        {
            lstAction = lstAction.Where(x => x.name.Contains(request.Name) || x.code.Contains(request.Name) || x.description.Contains(request.Name));
        }

        if (request.FunctionId != null)
        {
            lstAction = lstAction.Where(x => x.function_id == request.FunctionId);
        }

        int countElements = lstAction.Count();

        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        var dataList = await lstAction.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();
        var dataResult = new DataListResponse { PageNo = request.PageNo, PageSize = request.PageSize, TotalElements = countElements, TotalPage = totalPage, Data = dataList };
        
        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> UpdateAsync(ActionRequest request, string username)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        if (request.FunctionId == null)
        {
            return new APIResponse("ERROR_FUNC_ID_MISSING");
        }
        var data = await _context.Actions.Where(x => x.id == request.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        var dataCode = await _context.Actions.Where(x => x.code == request.Code && x.id != request.Id && x.function_id == request.FunctionId).FirstOrDefaultAsync();

        if (dataCode != null)
        {
            return new APIResponse("ERROR_CODE_EXISTS");
        }

        try
        {
            if (request.Code != null && request.Code.Length > 0)
            {
                data.code = request.Code;
            }

            if (request.Name != null && request.Name.Length > 0)
            {
                data.name = request.Name;
            }

            if (request.FunctionId != null)
            {
                data.function_id = (Guid)request.FunctionId;
            }

            data.url = request.Url;
            data.description = request.Description;
            data.is_default = (bool)request.IsDefault;
            if (request.Status != null)
            {
                data.status = (int)request.Status;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE_FAIL");
        }

        return new APIResponse(200);
    }
}
