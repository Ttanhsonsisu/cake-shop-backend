using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Auth;
using cake_shop_back_end.Models.auth;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Auth;

public class FunctionDataAccess(AppDbContext _context) : IFunction
{
    public async Task<APIResponse> ChangeStatusAsync(FunctionRequest req)
    {
        var data = await _context.Functions.Where(x => x.id == req.Id).FirstOrDefaultAsync();

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

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse(400);
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateAsync(FunctionRequest request, string username)
    {
        if (request.Code == null)
        {
            return new APIResponse("ERROR_CODE_MISSING");
        }

        if (request.Name == null)
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        if (request.Url == null)
        {
            return new APIResponse("ERROR_URL_MISSING");
        }

        if (request.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        try
        {
            var data = new Function();
            data.code = request.Code;
            data.name = request.Name;
            data.description = request.Description;
            data.url = request.Url;
            data.status = 1;
            data.is_default = request.IsDefault;
            data.user_created = username;
            data.user_updated = username;
            data.date_created = DateTime.Now;
            data.date_updated = DateTime.Now;

            await _context.Functions.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_ADD_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(FunctionRequest req)
    {
        var data = await _context.Functions.Where(x => x.id == req.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            _context.Functions.Remove(data);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse(400);
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetaiAsync(Guid id)
    {
        var Function = await (from p in _context.Functions
                        where p.id == id
                        select new
                        {
                            id = p.id,
                            code = p.code,
                            name = p.name,
                            status = p.status,
                            description = p.description,
                            url = p.url,
                            is_default = p.is_default
                        }).FirstOrDefaultAsync();

        if (Function == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        return new APIResponse(Function);
    }

    public async Task<APIResponse> GetFunctionTreeAsync()
    {
        var listFunctionsTree = await (from p in _context.Functions
                                 select new
                                 {
                                     id = p.id,
                                     code = p.code,
                                     name = p.name,
                                     status = p.status,
                                     description = p.description,
                                     url = p.url,
                                     list_action = (from i in _context.Actions
                                                    where i.function_id == p.id
                                                    select new
                                                    {
                                                        id = i.id,
                                                        code = i.code,
                                                        name = i.name,
                                                        status = i.status,
                                                        description = i.description,
                                                        url = i.url,
                                                        is_default = i.is_default,
                                                        function_id = i.function_id,
                                                        user_created = i.user_created,
                                                        user_updated = i.user_updated,
                                                        date_created = i.date_created,
                                                        date_updated = i.date_updated
                                                    }).ToList()
                                 }).ToListAsync();

        return new APIResponse(listFunctionsTree);
    }

    public async Task<APIResponse> GetListAsync(FunctionRequest request)
    {
        if (request.PageSize < 1)
        {
            request.PageSize = Consts.PAGE_SIZE;
        }

        if (request.PageNo < 1)
        {
            request.PageNo = 1;
        }

        int skipElements = (request.PageNo - 1) * request.PageSize;

        var lstFunction = (from p in _context.Functions
                           select new
                           {
                               id = p.id,
                               code = p.code,
                               name = p.name,
                               status = p.status,
                               description = p.description,
                               url = p.url,
                               is_default = p.is_default
                           });

        if (request.Name != null && request.Name.Length > 0)
        {
            lstFunction = lstFunction.Where(x => x.name.Contains(request.Name) || x.code.Contains(request.Name) || x.description.Contains(request.Name));
        }

        int countElements = lstFunction.Count();

        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        var dataList = await lstFunction.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();
        var dataResult = new DataListResponse { PageNo = request.PageNo, PageSize = request.PageSize, TotalElements = countElements, TotalPage = totalPage, Data = dataList };
        
        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> GetListFunctionPermissionAsync()
    {
        var listFunctions = await (from p in _context.Functions
                             where p.status == 1 
                             select new
                             {
                                 id = p.id,
                                 code = p.code,
                                 name = p.name,
                                 actions = (from i in _context.Actions
                                            where i.function_id == p.id
                                            select new
                                            {
                                                action_id = i.id,
                                                action_name = i.name
                                            }).ToList()
                             }).ToListAsync();

        return new APIResponse(listFunctions);
    }

    public async Task<APIResponse> UpdateAsync(FunctionRequest request, string username)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        var data = await _context.Functions.Where(x => x.id == request.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
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

            data.url = request.Url;
            data.description = request.Description;
            data.is_default = request.IsDefault;

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
