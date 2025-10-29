using cake_shop_back_end.DataObjects.Requests.Auth;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Auth;

public partial class UserDataAccess
{
    public async Task<APIResponse> ListStaffAsync(UserRequest request)
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

        var query = _context.Users.Where(p => p.is_admin == true);

        if (request.FullName != null && request.FullName.Length > 0)
        {
            query = query.Where(p => p.username.Contains(request.FullName) ||
                                     p.full_name.Contains(request.FullName) ||
                                     p.email.Contains(request.FullName) ||
                                     p.phone.Contains(request.FullName));
        }

        if (request.UserGroupId != null)
        {
            query = query.Where(p => p.user_group_id == request.UserGroupId);
        }

        if (request.Status != null)
        {
            query = query.Where(p => p.status == request.Status);
        }

        var dbQuery = (from p in query // 'query' đã được lọc
                       join f in _context.UserGroups on p.user_group_id equals f.id into fs
                       from f in fs.DefaultIfEmpty()
                       orderby p.date_created descending
                       select new
                       {
                           // Chọn dữ liệu thô (raw data)
                           id = p.id,
                           username = p.username,
                           full_name = p.full_name,
                           email = p.email,
                           phone = p.phone,
                           address = p.address,
                           user_group_id = p.user_group_id,
                           user_group_name = f != null ? f.name : "",
                           date_created = p.date_created, 
                           status = p.status
                       });

        int countElements = await dbQuery.CountAsync();

        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        var pagedData = await dbQuery.Skip(skipElements)
                                     .Take(request.PageSize)
                                     .ToListAsync();

        var dataList = pagedData.Select(p => new
        {
            id = p.id,
            username = p.username,
            full_name = p.full_name,
            email = p.email,
            phone = p.phone,
            address = p.address,
            user_group_id = p.user_group_id,
            user_group_name = p.user_group_name,
            date_created = p.date_created != null ? _commonFunction.ConvertDateToStringSort(p.date_created) : "",
            status = p.status
        }).ToList(); 

        var dataResult = new DataListResponse 
        { 
            PageNo = request.PageNo, 
            PageSize = request.PageSize,
            TotalElements = countElements,
            TotalPage = totalPage,
            Data = dataList
        };

        return new APIResponse(dataResult);
    }
}
