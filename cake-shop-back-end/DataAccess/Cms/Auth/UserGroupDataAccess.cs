using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Auth;
using cake_shop_back_end.Models.auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace cake_shop_back_end.DataAccess.Cms.Auth;

public class UserGroupDataAccess(AppDbContext _context) : IUserGroup
{
    public async Task<APIResponse> ChangeStatusAsync(UserGroupRequest req)
    {
        var data = await _context.UserGroups.Where(x => x.id == req.Id).FirstOrDefaultAsync();

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

    public async Task<APIResponse> CreateAsync(UserGroupRequest request, string username)
    {
        if (request.Name == null)
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        var dataSame = await _context.UserGroups.Where(x => x.name == request.Name).FirstOrDefaultAsync();

        if (dataSame != null)
        {
            return new APIResponse("ERROR_NAME_EXIST");
        }

        if (request.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var data = new UserGroup();
            data.name = request.Name;
            data.description = request.Description;
            data.status = 1;
            data.user_created = username;
            data.user_updated = username;
            data.date_created = DateTime.Now;
            data.date_updated = DateTime.Now;

            await _context.UserGroups.AddAsync(data);
            // Save Changes
            await _context.SaveChangesAsync();

            var userGroup = _context.UserGroups.OrderByDescending(x => x.date_created).FirstOrDefault();
            if (userGroup != null && request.UserGroupPermissions != null && request.UserGroupPermissions.Count > 0)
            {
                for (int i = 0; i < request.UserGroupPermissions.Count; i++)
                {
                    var item = new UserGroupPermission();
                    item.user_group_id = userGroup.id;
                    item.action_id = request.UserGroupPermissions[i].action_id;
                    _context.UserGroupPermissions.Add(item);
                }

                await _context.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();

            return new APIResponse("ERROR_ADD_FAIL");
        }

        await transaction.CommitAsync();
        await transaction.DisposeAsync();

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(UserGroupRequest req)
    {
        var data = await _context.UserGroups.Where(x => x.id == req.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var lstPermissions = await _context.UserGroupPermissions.Where(x => x.user_group_id == req.Id).ToListAsync();
            
            _context.UserGroupPermissions.RemoveRange(lstPermissions);

            await _context.SaveChangesAsync();

            _context.UserGroups.Remove(data);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {

            await transaction.RollbackAsync();

            return new APIResponse(400);
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        List<object> userGroupPermissions = new List<object>();
        var action = await (from p in _context.UserGroups
                      where p.id == id
                      select new
                      {
                          id = p.id,
                          name = p.name,
                          description = p.description,
                          status = p.status,
                          user_created = p.user_created,
                          user_updated = p.user_updated,
                          date_created = p.date_created,
                          date_updated = p.date_updated,
                          userGroupPermissions = (from i in _context.UserGroupPermissions
                                                  join f in _context.Actions on i.action_id equals f.id into fs
                                                  from f in fs.DefaultIfEmpty()
                                                  join g in _context.Functions on f.function_id equals g.id into gs
                                                  from g in gs.DefaultIfEmpty()
                                                  where i.user_group_id == p.id
                                                  select new
                                                  {
                                                      action_id = i.action_id,
                                                      action_name = f == null ? "" : f.name,
                                                      //function_id = g == null ? null : g.id,
                                                      function_name = g == null ? null : g.name
                                                  }).ToList()
                      }).FirstOrDefaultAsync();

        if (action == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        return new APIResponse(action);
    }

    public async Task<APIResponse> GetListAsync(UserGroupRequest request)
    {
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

        var lstUserGroup = (from p in _context.UserGroups
                            orderby p.date_created descending
                            select new
                            {
                                id = p.id,
                                name = p.name,
                                description = p.description,
                                status = p.status,
                                user_created = p.user_created,
                                user_updated = p.user_updated,
                                date_created = p.date_created,
                                date_updated = p.date_updated
                            });

        // Nếu tồn tại Where theo tên
        if (request.Name != null && request.Name.Length > 0)
        {
            lstUserGroup = lstUserGroup.Where(x => x.name.Contains(request.Name));
        }

        int countElements = lstUserGroup.Count();

        // Số lượng trang
        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        // Data Sau phân trang
        var dataList = await lstUserGroup.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();

        var dataResult = new DataListResponse { PageNo = request.PageNo, PageSize = request.PageSize, TotalElements = countElements, TotalPage = totalPage, Data = dataList };
        
        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> GetPermissionAsync(Guid id)
    {
        List<object> userGroupPermissions = new List<object>();
        var action = await (from p in _context.UserGroups
                      where p.id == id
                      select new
                      {
                          id = p.id,
                          name = p.name,
                          userGroupPermissions = (from i in _context.UserGroupPermissions
                                                  join f in _context.Actions on i.action_id equals f.id into fs
                                                  from f in fs.DefaultIfEmpty()
                                                  join g in _context.Functions on f.function_id equals g.id into gs
                                                  from g in gs.DefaultIfEmpty()
                                                  where i.user_group_id == p.id
                                                  select new
                                                  {
                                                      action_id = i.action_id,
                                                      action_name = f == null ? "" : f.name,
                                                      function_id = g.id,
                                                      function_name = g == null ? null : g.name
                                                  }).ToList()
                      }).FirstOrDefaultAsync();

        if (action == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        return new APIResponse(action);
    }

    public async Task<APIResponse> UpdateAsync(UserGroupRequest request, string username)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        var data = await _context.UserGroups.Where(x => x.id == request.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {

            if (request.Name != null && request.Name.Length > 0)
            {
                data.name = request.Name;
                var dataSame = await _context.UserGroups.Where(x => x.name == request.Name && x.id != request.Id ).FirstOrDefaultAsync();

                if (dataSame != null)
                {
                    return new APIResponse("ERROR_CODE_EXIST");
                }

            }

            data.description = request.Description;
            if (request.Status != null)
            {
                data.status = (int)request.Status;
            }

            var lstPermissionDeletes = await _context.UserGroupPermissions.Where(x => x.user_group_id == data.id).ToListAsync();
            
            _context.UserGroupPermissions.RemoveRange(lstPermissionDeletes);

            if (data != null && request.UserGroupPermissions != null && request.UserGroupPermissions.Count > 0)
            {
                for (int i = 0; i < request.UserGroupPermissions.Count; i++)
                {
                    var item = new UserGroupPermission();
                    item.user_group_id = data.id;
                    item.action_id = request.UserGroupPermissions[i].action_id;
                    await _context.UserGroupPermissions.AddAsync(item);
                }
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new APIResponse("ERROR_UPDATE_FAIL");
        }

        return new APIResponse(200);
    }
}
