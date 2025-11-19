using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Customer;

public class CustomerDataAccess(AppDbContext _context, ICommonFunction _commonFunction) : ICustomer
{
    public async Task<APIResponse> ChangeAvatar(CustomerRequest req, string username)
    {
        // validate input
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_INVALID");
        }
        if (req.Id == null || req.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_CUSTOMER_ID_INVALID");
        }
        if (string.IsNullOrEmpty(req.Avatar))
        {
            return new APIResponse("ERROR_CUSTOMER_AVATAR_INVALID");
        }
        var data = await _context.Customers.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_CUSTOMER_NOT_FOUND");
        }
        var user = await _context.Users.FindAsync(data.user_id);

        try
        {
            user.avatar = (string)req.Avatar;
            user.user_updated = username;
            user.date_updated = DateTime.Now;

            data.user_updated = username;
            data.date_updated = DateTime.Now;

            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("ERROR_UPDATE_CUSTOMER_AVATAR_FAILED");
        }

        return new APIResponse(200);

    }

    public async Task<APIResponse> ChangePassword(Guid idUser, PasswordRequest req, string username)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_INVALID");
        }
        if (idUser == Guid.Empty || idUser == null)
        {
            return new APIResponse("ERROR_USER_ID_INVALID");
        }
        if (req.OldPassword == null || string.IsNullOrEmpty(req.OldPassword))
        {
            return new APIResponse("ERROR_OLD_PASSWORD_INVALID");
        }
        if (req.NewPassword == null || string.IsNullOrEmpty(req.NewPassword))
        {
            return new APIResponse("ERROR_NEW_PASSWORD_INVALID");
        }
        var user = await _context.Users.FindAsync(idUser);
        if (user == null)
        {
            return new APIResponse("ERROR_USER_NOT_FOUND");
        }

        // check old password
        var checkOldPassword = user.password == _commonFunction.ComputeSha256Hash(req.OldPassword);
        if (!checkOldPassword)
        {
            return new APIResponse("ERROR_OLD_PASSWORD_INCORRECT");
        }
        try
        {
            user.password = _commonFunction.ComputeSha256Hash(req.NewPassword);
            user.user_updated = username;
            user.date_updated = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("ERROR_CHANGE_PASSWORD_FAILED");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> ChangeStatusAsync(CustomerRequest req)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_INVALID");
        }
        if (req.Id == null || req.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_CUSTOMER_ID_INVALID");
        }
        var data = await _context.Customers.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_CUSTOMER_NOT_FOUND");
        }
        try
        {
            data.status = req.Status;
            data.date_updated = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("ERROR_CHANGE_CUSTOMER_STATUS_FAILED");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateAsync(CustomerRequest request)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_INVALID");
        }
        // check email exist
        var checkDataExist = _context.Users.Any(u => u.email == request.Email || u.phone == request.Phone);
        if (checkDataExist)
        {
            return new APIResponse("ERROR_EMAIL_ALREADY_EXIST");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Guid userId = Guid.NewGuid();
            Guid customerId = Guid.NewGuid();

            var userInsert = new cake_shop_back_end.Models.auth.User();
            userInsert.id = userId;
            userInsert.user_group_id = request.UserGroupId;
            userInsert.email = request.Email;
            userInsert.full_name = request.Name;
            userInsert.is_admin = false;
            userInsert.is_sysadmin = false;
            userInsert.customer_id = customerId;
            userInsert.phone = request.Phone;
            userInsert.username = request.Email;
            userInsert.password = _commonFunction.ComputeSha256Hash(request.Password ?? "12345678");
            userInsert.status = request.Status;
            userInsert.is_delete = false;
            userInsert.date_created = DateTime.Now;
            userInsert.user_created = "ADMIN";
            await _context.Users.AddAsync(userInsert);

            var dataInsert = new cake_shop_back_end.Models.auth.Customer();
            dataInsert.id = customerId;
            dataInsert.email = request.Email;
            dataInsert.phone = request.Phone;
            dataInsert.status = request.Status;
            dataInsert.name = request.Name;
            dataInsert.birth_date = request.BirthDate;
            dataInsert.gender = request.Gender;
            dataInsert.user_id = userId;
            dataInsert.date_created = DateTime.Now;
            dataInsert.user_created = "ADMIN";
            await _context.Customers.AddAsync(dataInsert);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return new APIResponse("ERROR_CREATE_CUSTOMER_FAILED");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(CustomerRequest req)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_INVALID");
        }
        if (req.Id == null || req.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_CUSTOMER_ID_INVALID");
        }
        var data = await _context.Customers.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_CUSTOMER_NOT_FOUND");
        }
        var user = await _context.Users.FindAsync(data.user_id);
        if (user == null)
        {
            return new APIResponse("ERROR_USER_NOT_FOUND");
        }

        try
        {
            user.is_delete = true;
            data.status = 0;
            data.date_updated = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("ERROR_DELETE_CUSTOMER_FAILED");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        if (id == Guid.Empty || id == null)
        {
            return new APIResponse("ERROR_CUSTOMER_ID_INVALID");
        }
        var data = await _context.Customers.FindAsync(id);
        if (data == null)
        {
            return new APIResponse("ERROR_CUSTOMER_NOT_FOUND");
        }

        return new APIResponse(200)
        {
            Data = data
        };
    }

    public Task<APIResponse> GetInfoCustomerWithIdUser(Guid idUser)
    {
        throw new NotImplementedException();
    }

    public Task<APIResponse> GetInforSettingAccountCustomer(Guid idUser)
    {
        throw new NotImplementedException();
    }

    public async Task<APIResponse> GetListAsync(CustomerRequest request)
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

        var query = _context.Customers.AsQueryable();

        if (request.Name != null && request.Name.Length > 0)
        {
            string keywordPattern = $"%{request.Name}%";
            query = query.Where(x =>
                (x.name != null && EF.Functions.Like(x.name, keywordPattern)) 
            );
        }

        if (request.Status != null)
        {
            query = query.Where(x => x.status == request.Status);
        }


        int countElements = await query.CountAsync();

        int totalPage = countElements > 0
            ? (int)Math.Ceiling(countElements / (double)request.PageSize)
            : 0;

        var data = await query
            //.OrderBy(x => x.orders)
            //.ThenBy(x => x.name)
            .Skip(skipElement)
            .Take(request.PageSize)
            .Select(
            d => new
            {
                d.id,
                d.name,
                d.status,
                d.email,
                d.phone,
                d.birth_date,
                d.gender,
                d.date_created,
                d.date_updated,
                role = _context.Users
                    .Where(u => u.id == d.user_id)
                    .Select(u => new
                    {
                        u.user_group_id,
                        u.is_admin,
                        u.is_sysadmin
                    }).FirstOrDefault()
            })
            .ToListAsync();

        var dataResult = new DataListResponse
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalPage = totalPage,
            Data = data
        };

        return new APIResponse(dataResult);
    }

    public Task<APIResponse> UpdateAccountSettingAsync(Guid idUser, CustomerRequest request, string username)
    {
        throw new NotImplementedException();
    }

    public Task<APIResponse> UpdateAsync(CustomerRequest request, string username)
    {
        throw new NotImplementedException();
    }

    public Task<APIResponse> UpdateBillingAddressAsync(CustomerRequest request, string username)
    {
        throw new NotImplementedException();
    }

    public Task<APIResponse> UpdateBillingAddressAsync(Guid idUser, CustomerRequest request, string username)
    {
        throw new NotImplementedException();
    }

    public Task<APIResponse> UpdateNomalInfo(CustomerRequest request, string username)
    {
        throw new NotImplementedException();
    }
}
