using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using cake_shop_back_end.Models.auth;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Auth;

public class UserDataAcess(AppDbContext _context, ICommonFunction _commonFunction) : IUser
{
    public async Task<APIResponse> ChangeStatusAsync(UserRequest req)
    {
        var data = await _context.Users.Where(x => x.id == req.Id).FirstOrDefaultAsync();

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
            data.status = req.Status;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse(400);
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateAsync(UserRequest request, string username)
    {
        if (request.Username == null)
        {
            return new APIResponse("ERROR_USERNAME_MISSING");
        }

        var dataSame = await _context.Users.Where(x => x.username == request.Username && x.is_admin == true).FirstOrDefaultAsync();

        if (dataSame != null)
        {
            return new APIResponse("ERROR_USERNAME_EXIST");
        }

        if (request.FullName == null)
        {
            return new APIResponse("ERROR_FULLNAME_MISSING");
        }

        if (request.Email == null)
        {
            return new APIResponse("ERROR_EMAIL_MISSING");
        }
        // check duplicate email
        var dataSameEmail = await _context.Users.Where(x => x.email == request.Email && x.is_admin == true).FirstOrDefaultAsync();

        if (dataSameEmail != null)
        {
            return new APIResponse("ERROR_EMAIL_EXISTS");
        }

        if (request.Phone == null)
        {
            return new APIResponse("ERROR_PHONE_MISSING");
        }

        if (request.Password == null)
        {
            return new APIResponse("ERROR_PASSWORD_MISSING");
        }

        if (request.UserGroupId == null)
        {
            return new APIResponse("ERROR_USERGROUP_ID_MISSING");
        }

        if (request.UserPermissions == null || request.UserPermissions.Count == 0)
        {
            return new APIResponse("ERROR_PERMISSION_MISSING");
        }

        if (request.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }


        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var data = new User();
            data.full_name = request.FullName;
            data.avatar = request.Avatar;
            data.address = request.Address;
            data.user_group_id = request.UserGroupId;
            data.email = request.Email;
            data.phone = request.Phone;
            data.username = request.Username;
            data.password = _commonFunction.ComputeSha256Hash(request.Password);
            data.status = 1;
            data.is_sysadmin = false;
            data.is_admin = true;
            data.user_created = username;
            data.user_updated = username;
            data.date_created = DateTime.Now;
            data.date_updated = DateTime.Now;
            
            await _context.Users.AddAsync(data);
            // Save Changes
            await _context.SaveChangesAsync();

            var user = _context.Users.OrderByDescending(x => x.date_created).FirstOrDefault();
            if (user != null && request.UserPermissions != null && request.UserPermissions.Count > 0)
            {
                for (int i = 0; i < request.UserPermissions.Count; i++)
                {
                    var item = new UserPermission();
                    item.user_id = user.id;
                    item.action_id = request.UserPermissions[i].action_id;

                    await _context.UserPermissions.AddAsync(item);
                }

                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            await transaction.DisposeAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();

            return new APIResponse("ERROR_ADD_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateNomalUser(UserRequest request)
    {
        if (request.Username == null)
        {
            return new APIResponse("ERROR_USERNAME_MISSING");
        }

        var dataSame = await _context.Users.Where(x => x.username == request.Username).FirstOrDefaultAsync();

        if (dataSame != null)
        {
            return new APIResponse("ERROR_USERNAME_EXIST");
        }

        if (request.FullName == null)
        {
            return new APIResponse("ERROR_FULLNAME_MISSING");
        }

        if (request.Email == null)
        {
            return new APIResponse("ERROR_EMAIL_MISSING");
        }
        // check duplicate email
        var dataSameEmail = await _context.Users.Where(x => x.email == request.Email).FirstOrDefaultAsync();

        if (dataSameEmail != null)
        {
            return new APIResponse("ERROR_EMAIL_EXISTS");
        }

        if (request.Phone == null)
        {
            return new APIResponse("ERROR_PHONE_MISSING");
        }

        if (request.Password == null)
        {
            return new APIResponse("ERROR_PASSWORD_MISSING");
        }

        if (request.UserGroupId == null)
        {
            return new APIResponse("ERROR_USERGROUP_ID_MISSING");
        }

        if (request.UserPermissions == null || request.UserPermissions.Count == 0)
        {
            return new APIResponse("ERROR_PERMISSION_MISSING");
        }

        if (request.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }


        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var data = new User();
            data.full_name = request.FullName;
            data.avatar = request.Avatar;
            data.address = request.Address;
            data.email = request.Email;
            data.phone = request.Phone;
            data.username = request.Username;
            data.password = _commonFunction.ComputeSha256Hash(request.Password);
            data.status = 1;
            data.is_sysadmin = false;
            data.is_admin = false;
            data.date_created = DateTime.Now;
            data.date_updated = DateTime.Now;

            await _context.Users.AddAsync(data);
            // Save Changes
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            await transaction.DisposeAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();

            return new APIResponse("ERROR_ADD_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(UserRequest req)
    {
        var data = await _context.Users.Where(x => x.id == req.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        if (data.username == "administrator")
        {
            return new APIResponse("ERROR_DELETE_ADMIN");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var lstPermissions = await _context.UserPermissions.Where(x => x.user_id == req.Id).ToListAsync();

            _context.UserPermissions.RemoveRange(lstPermissions);

            await _context.SaveChangesAsync();

            _context.Users.Remove(data);

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
        List<object> userPermissions = new List<object>();
        var action = await (from p in _context.Users
                      where p.id == id && p.is_admin == true
                      select new
                      {
                          id = p.id,
                          username = p.username,
                          full_name = p.full_name,
                          avatar = p.avatar,
                          address = p.address,
                          user_group_id = p.user_group_id,
                          email = p.email,
                          phone = p.phone,
                          status = p.status,
                          userPermissions = (from i in _context.UserPermissions
                                             join f in _context.Actions on i.action_id equals f.id into fs
                                             from f in fs.DefaultIfEmpty()
                                             join g in _context.Functions on f.function_id equals g.id into gs
                                             from g in gs.DefaultIfEmpty()
                                             where i.user_id == p.id
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

    public async Task<APIResponse> GetListAsync(UserRequest request)
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

        var lstUser = (from p in _context.Users
                       join f in _context.UserGroups on p.user_group_id equals f.id into fs
                       from f in fs.DefaultIfEmpty()
                       where p.is_admin == true
                       orderby p.date_created descending
                       select new
                       {
                           id = p.id,
                           username = p.username,
                           full_name = p.full_name,
                           email = p.email,
                           phone = p.phone,
                           address = p.address,
                           user_group_id = p.user_group_id,
                           user_group_name = f != null ? f.name : "",
                           date_created = p.date_created != null ? _commonFunction.ConvertDateToStringSort(p.date_created) : "",
                           status = p.status
                       });

        if (request.FullName != null && request.FullName.Length > 0)
        {
            lstUser = lstUser.Where(x => x.username.Contains(request.FullName) || x.full_name.Contains(request.FullName) || x.email.Contains(request.FullName) || x.phone.Contains(request.FullName));
        }

        if (request.UserGroupId != null)
        {
            lstUser = lstUser.Where(x => x.user_group_id == request.UserGroupId);
        }

        if (request.Status != null)
        {
            lstUser = lstUser.Where(x => x.status == request.Status);
        }

        int countElements = lstUser.Count();
     
        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;
     
        var dataList = await lstUser.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();
        var dataResult = new DataListResponse { PageNo = request.PageNo, PageSize = request.PageSize, TotalElements = countElements, TotalPage = totalPage, Data = dataList };
        
        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> UpdateAsync(UserRequest request, string username)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        if (request.FullName == null)
        {
            return new APIResponse("ERROR_FULLNAME_MISSING");
        }

        if (request.Email == null)
        {
            return new APIResponse("ERROR_EMAIL_MISSING");
        }
        // check duplicate email
        var dataSameEmail = await _context.Users.Where(x => x.email == request.Email).FirstOrDefaultAsync();

        var user = _context.Users.Where(x => x.id == request.Id).FirstOrDefault();

        if (dataSameEmail != null && user.email != request.Email)
        {
            return new APIResponse("ERROR_EMAIL_EXISTS");
        }

        if (request.Phone == null)
        {
            return new APIResponse("ERROR_PHONE_MISSING");
        }

        if (request.UserGroupId == null)
        {
            return new APIResponse("ERROR_USERGROUP_ID_MISSING");
        }

        if (request.UserPermissions == null || request.UserPermissions.Count == 0)
        {
            return new APIResponse("ERROR_PERMISSION_MISSING");
        }

        var data = await _context.Users.Where(x => x.id == request.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }


        using var transaction = _context.Database.BeginTransaction();
        try
        {
            data.full_name = request.FullName;
            data.avatar = request.Avatar;
            data.address = request.Address;
            data.user_group_id = request.UserGroupId;
            data.email = request.Email;
            data.phone = request.Phone;

            if (request.Password != null && request.Password.Length > 0)
            {
                data.password = _commonFunction.ComputeSha256Hash(request.Password);
            }

            if (request.Status != null)
            {
                data.status = request.Status;
            }

            var lstPermissionDeletes = await _context.UserPermissions.Where(x => x.user_id == data.id).ToListAsync();

            _context.UserPermissions.RemoveRange(lstPermissionDeletes);

            if (data != null && request.UserPermissions != null && request.UserPermissions.Count > 0)
            {
                for (int i = 0; i < request.UserPermissions.Count; i++)
                {
                    var item = new UserPermission();
                    item.user_id = data.id;
                    item.action_id = request.UserPermissions[i].action_id;

                    await _context.UserPermissions.AddAsync(item);
                }
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            await transaction.DisposeAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();

            return new APIResponse("ERROR_UPDATE_FAIL");
        }

        return new APIResponse(200);
    }
}
