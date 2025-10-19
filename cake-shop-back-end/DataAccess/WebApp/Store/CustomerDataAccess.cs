using cake_shop_back_end.Data;
using cake_shop_back_end.DataAccess.Cms.Auth;
using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using cake_shop_back_end.Models.auth;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.WebApp.Store;

public class CustomerDataAccess(AppDbContext _context , ICommonFunction _commonFunctions) : ICustomer
{
    public async Task<APIResponse> ChangeStatusAsync(CustomerRequest req)
    {
        if (req.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        var data = await _context.Customers.Where(x => x.id == req.Id).FirstOrDefaultAsync();

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

    public async Task<APIResponse> CreateAsync(CustomerRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        if (string.IsNullOrEmpty(request.Email))
        {
            return new APIResponse("ERROR_EMAIL_MISSING");
        }

        if (request.BirthDate == null)
        {
            return new APIResponse("ERROR_BIRTHDATE_MISSING");
        }

        if (string.IsNullOrEmpty(request.Address))
        {
            return new APIResponse("ERROR_ADDRESS_MISSING");
        }

        if (string.IsNullOrEmpty(request.ZipCode))
        {
            return new APIResponse("ERROR_ZIP_CODE_MISSING");
        }

        if (request.Gender == null)
        {
            return new APIResponse("ERROR_GENDER_MISSING");
        }

        if (request.CountryId == null)
        {
            return new APIResponse("ERROR_COUNTRY_ID_MISSING");
        }

        if (request.StatesId == null)
        {
            return new APIResponse("ERROR_STATES_ID_MISSING");
        }

        try
        {
            var data = new Customer();
            data.id = Guid.NewGuid();
            data.user_id = (Guid)request.UserId;
            data.name = request.Name;
            data.email = request.Email;
            data.birth_date = request.BirthDate;
            data.address = request.Address;
            data.zip_code = request.ZipCode;
            data.gender = request.Gender;
            data.country_id = request.CountryId;
            data.states_id = request.StatesId;
            data.company_name = request.CompanyName;
            data.status = 1;
            data.date_created = DateTime.Now;
            data.date_updated = DateTime.Now;
            data.user_created = "NEW CUSTOMER";
            data.user_updated = "NEW CUSTOMER";

            await _context.Customers.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_ADD_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(CustomerRequest req)
    {
        if (req.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        var data = await _context.Customers.Where(x => x.id == req.Id).FirstOrDefaultAsync();
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }
        try
        {
            _context.Customers.Remove(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_DELETE_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        var data = await _context.Customers.Where(x => x.id == id).FirstOrDefaultAsync();

        var dataResult = await (
            from b in _context.Customers
            where b.id == id
            select new
            {
                id = b.id,
                user_id = b.user_id,
                name = b.name,
                phone = b.phone,
                status = b.status,
                address = b.address,
                country = new
                {
                    id = b.country_id,
                    name = b.country_id != null ? _context.Provinces.Where(c => c.id == b.country_id).Select(c => c.name).FirstOrDefault() : ""
                },
                states = new
                {
                    id = b.states_id,
                    name = b.states_id != null ? _context.Provinces.Where(c => c.id == b.states_id).Select(c => c.name).FirstOrDefault() : ""
                },
                b.zip_code,
                email = b.email,
                birth_date = b.birth_date
            }).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        return new APIResponse(200, dataResult);
    }

    public async Task<APIResponse> GetListAsync(CustomerRequest request)
    {
        if (request.PageSize < 1)
        {
            request.PageSize = Consts.PAGE_SIZE;
        }

        if (request.PageNo < 1)
        {
            request.PageSize = 1;
        }
        // Số lượng Skip
        int skipElements = (request.PageNo - 1) * request.PageSize;

        var lstCustomer = (
            from b in _context.Customers
            select new
            {
                id = b.id,
                user_id = b.user_id,
                name = b.name,
                phone = b.phone,
                status = b.status,
                address = b.address,
                b.country_id,
                country_name = b.country_id != null ? _context.Provinces.Where(c => c.id == b.country_id).Select(c => c.name).FirstOrDefault() : "",
                states_name = b.states_id != null ? _context.Provinces.Where(c => c.id == b.states_id).Select(c => c.name).FirstOrDefault() : "",
                b.states_id,
                b.zip_code,
                email = b.email,
                birth_date = b.birth_date
            });

        if (request.Name != null && request.Name?.Length > 0)
        {
            lstCustomer = lstCustomer.Where(x => x.name.Contains(request.Name) || x.email.Contains(request.Name) || x.phone.Contains(request.Name));
        }

        if (request.Status != null)
        {
            lstCustomer = lstCustomer.Where(x => x.status == request.Status);
        }

        int countElements = lstCustomer.Count();

        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        var dataList = await lstCustomer.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();
        var dataResult = new DataListResponse { PageNo = request.PageNo, PageSize = request.PageSize, TotalElements = countElements, TotalPage = totalPage, Data = dataList };

        return new APIResponse(dataResult);
    }

    public async Task<APIResponse> UpdateAccountSettingAsync(Guid idUser ,CustomerRequest request, string username)
    {
        if (idUser == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        var data = await _context.Users.Where(x => x.id == idUser).FirstOrDefaultAsync();
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }
        try
        {
            data.full_name = request.Name;
            data.email = request.Email;
            data.phone = request.Phone;
            data.date_updated = DateTime.Now;
            data.user_updated = username;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE_FAIL");
        }
        return new APIResponse(200);
    }

    public async Task<APIResponse> UpdateAsync(CustomerRequest request, string username)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        if (string.IsNullOrEmpty(request.Email))
        {
            return new APIResponse("ERROR_EMAIL_MISSING");
        }

        if (string.IsNullOrEmpty(request.Phone))
        {
            return new APIResponse("ERROR_PHONE_MISSING");
        }

        if (request.BirthDate == null)
        {
            return new APIResponse("ERROR_BIRTHDATE_MISSING");
        }

        if (request.Gender == null)
        {
            return new APIResponse("ERROR_GENDER_MISSING");
        }

        if (request.CountryId == null)
        {
            return new APIResponse("ERROR_COUNTRY_ID_MISSING");
        }

        if (request.StatesId == null)
        {
            return new APIResponse("ERROR_STATES_ID_MISSING");
        }

        if (string.IsNullOrEmpty(request.ZipCode))
        {
            return new APIResponse("ERROR_ZIP_CODE_MISSING");
        }

        if (request.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        if (string.IsNullOrEmpty(request.Address))
        {
            return new APIResponse("ERROR_ADDRESS_MISSING");
        }

        if (string.IsNullOrEmpty(request.CompanyName))
        {
            return new APIResponse("ERROR_COMPANY_NAME_MISSING");
        }

        var data = await _context.Customers.Where(x => x.id == request.Id).FirstOrDefaultAsync();
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            data.name = request.Name;
            data.email = request.Email;
            data.phone = request.Phone;
            data.birth_date = request.BirthDate;
            data.gender = request.Gender;
            data.country_id = request.CountryId;
            data.states_id = request.StatesId;
            data.zip_code = request.ZipCode;
            data.status = request.Status;
            data.address = request.Address;
            data.company_name = request.CompanyName;
            data.date_updated = DateTime.Now;
            data.user_updated = username;

            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> UpdateBillingAddressAsync(CustomerRequest request, string username)
    {
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        if (request.Name == null || request.Name.Length == 0)
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }

        if (request.Address == null || request.Address.Length == 0)
        {
            return new APIResponse("ERROR_ADDRESS_MISSING");
        }

        if (request.CountryId == null)
        {
            return new APIResponse("ERROR_COUNTRY_ID_MISSING");
        }

        var idCountry = await _context.Provinces.Where(x => x.id == request.CountryId).FirstOrDefaultAsync();
        if (idCountry == null)
        {
            return new APIResponse("ERROR_COUNTRY_ID_NOT_EXISTS");
        }
        var idStates = await _context.Provinces.Where(x => x.id == request.StatesId).FirstOrDefaultAsync();
        if (idStates == null)
        {
            return new APIResponse("ERROR_STATES_ID_NOT_EXISTS");
        }

        if (request.Email == null || request.Email.Length == 0)
        {
            return new APIResponse("ERROR_EMAIL_MISSING");
        }

        if (request.Phone == null || request.Phone.Length == 0)
        {
            return new APIResponse("ERROR_PHONE_MISSING");
        }

        var data = await _context.Customers.Where(x => x.id == request.Id).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            data.name = request.Name;
            data.email = request.Email;
            data.phone = request.Phone;
            data.address = request.Address;
            data.country_id = request.CountryId;
            data.states_id = request.StatesId;
            data.zip_code = request.ZipCode ?? data.zip_code;
            data.company_name = request.CompanyName ?? data.company_name;
            data.date_updated = DateTime.Now;
            data.user_updated = username;
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE_FAIL");
        }

        return new APIResponse(200);
    }

    public Task<APIResponse> UpdateNomalInfo(CustomerRequest request, string username)
    {
        throw new NotImplementedException();
    }

    public async Task<APIResponse> GetInforSettingAccountCustomer(Guid idUser)
    {
        if (idUser == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }

        var dataCheckExits = await _context.Customers.Where(x => x.user_id == idUser).FirstOrDefaultAsync();
        if (dataCheckExits == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        var dataResult = await _context.Users.Where(x => x.id == idUser)
            .Select(x => new
            {
                username = x.username,
                name = x.full_name,
                avatar = x.avatar,
                email = x.email,
                phone = x.phone,
                date_created = x.date_created,
                date_updated = x.date_updated
            }).FirstOrDefaultAsync();

        return new APIResponse(200, dataResult);
    }

    public async Task<APIResponse> ChangeAvatar(CustomerRequest req, string username)
    {
        if (req.UserId == null)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        var data = await _context.Users.Where(x => x.id == req.UserId).FirstOrDefaultAsync();
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }
        if (string.IsNullOrEmpty(req.Avatar))
        {
            return new APIResponse("ERROR_AVATAR_MISSING");
        }
        try
        {
            data.avatar = req.Avatar;
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

    public async Task<APIResponse> GetInfoCustomerWithIdUser(Guid idUser)
    {
        if (idUser == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        var data = await _context.Customers.Where(x => x.user_id == idUser)
            .Select(x => new
            {
                id = x.id,
                user_id = x.user_id,
                name = x.name,
                phone = x.phone,
                status = x.status,
                address = x.address,
                country = new
                {
                    id = x.country_id,
                    name = x.country_id != null ? _context.Provinces.Where(c => c.id == x.country_id).Select(c => c.name).FirstOrDefault() : ""
                },
                states = new
                {
                    id = x.states_id,
                    name = x.states_id != null ? _context.Provinces.Where(c => c.id == x.states_id).Select(c => c.name).FirstOrDefault() : ""
                },
                x.zip_code,
                email = x.email,
                birth_date = x.birth_date
            }).FirstOrDefaultAsync();
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        return new APIResponse(200, data);

    }

    public async Task<APIResponse> UpdateBillingAddressAsync(Guid idUser, CustomerRequest request, string username)
    {
        if (idUser == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        var data = await _context.Customers.Where(x => x.user_id == idUser).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }
        try
        {
            data.name = request.Name;
            data.email = request.Email;
            data.phone = request.Phone;
            data.address = request.Address;
            data.country_id = request.CountryId;
            data.states_id = request.StatesId;
            data.zip_code = request.ZipCode ?? data.zip_code;
            data.company_name = request.CompanyName ?? data.company_name;
            data.date_updated = DateTime.Now;
            data.user_updated = username;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> ChangePassword(Guid idUser , PasswordRequest req , string username)
    {
        if (idUser == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        var data = await _context.Users.Where(x => x.id == idUser).FirstOrDefaultAsync();

        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }
        
        var checkOldPassword = data.password == _commonFunctions.ComputeSha256Hash(req.OldPassword);
        if (!checkOldPassword)
        {
            return new APIResponse("ERROR_OLD_PASSWORD_INCORRECT");
        }

        try
        {

            data.password = _commonFunctions.ComputeSha256Hash(req.NewPassword);
            data.user_updated = username;
            data.date_updated = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_CHANGE_PASSWORD_FAIL");
        }
        return new APIResponse(200);

    }
}
