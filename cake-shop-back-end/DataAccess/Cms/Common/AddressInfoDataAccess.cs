//using cake_shop_back_end.Data;
//using cake_shop_back_end.DataObjects.Requests.Auth;
//using cake_shop_back_end.DataObjects.Responses;
//using cake_shop_back_end.Interfaces.Common;
//using Microsoft.EntityFrameworkCore;

//namespace cake_shop_back_end.DataAccess.Cms.Common;

//public class AddressInfoDataAccess(AppDbContext _context) : IAddressInfo
//{
//    public async Task<APIResponse> ChangeStatusAsync(AddressInfoRequest req, string username)
//    {
//        // validate data request in data
//        if (req == null)
//        {
//            return new APIResponse("ERROR_REQUEST_NOT_EXISTS");
//        }

//        if (string.IsNullOrEmpty(username))
//        {

//        }

//        if (req.id == null) 
//        {
        
//        }

//        if (req.status == null)
//        {

//        }

//        var data = await _context.AddressInfos.Where(v => v.id == req.id).FirstOrDefaultAsync();

//        if (data == null)
//        {
//            return new APIResponse("ERROR_ADDRESS_ID_NOT_EXISTS");
//        }

//        try
//        {
//            // UPDATE status
//            data.status = req.status;

//            await _context.SaveChangesAsync();

//        } catch (Exception ex)
//        {
//            return new APIResponse("ERROR_UPDATE_FAIL" +  ex.Message.ToUpper());
//        }
 
//        return new APIResponse("200");
//    }

//    public Task<APIResponse> CreateAsync(AddressInfoRequest request, string username)
//    {
//        // validate value
//        //// code check
//        // check exists // check duplicate
//        var data = _context.AddressInfos.Where();
//    }

//    public Task<APIResponse> DeleteAsync(AddressInfoRequest req, string username)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<APIResponse> GetDetailAsync(Guid id)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<APIResponse> GetListAddressInCustomer(Guid idCustomer, AddressInfoRequest request)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<APIResponse> GetListAllAsync(AddressInfoRequest request)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<APIResponse> UpdateAsync(AddressInfoRequest request, string username)
//    {
//        throw new NotImplementedException();
//    }
//}
