using Azure.Core;
using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Common;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Common;

public class LoggingDataAccess(AppDbContext _context) : ILogging
{
    public async Task<APIResponse> GetListAction(FilterLoggingRequest request)
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
        var lstOtherListType = (from p in _context.Loggings
                                where p.is_login == false && p.is_call_api == true
                                orderby p.date_created descending
                                select new
                                {
                                    user_created = p.user_created,
                                    functions = p.functions,
                                    actions = p.actions,
                                    IP = p.IP,
                                    is_login = p.is_login,
                                    is_call_api = p.is_call_api,
                                    date_created = p.date_created,
                                    result_logging = p.result_logging
                                });

        // Nếu tồn tại Where theo tên
        if (request.Search != null && request.Search.Length > 0)
        {
            lstOtherListType = lstOtherListType.Where(x => x.user_created.Contains(request.Search) || x.actions.Contains(request.Search));
        }

        if (request.Functions != null && request.Functions.Length > 0)
        {
            lstOtherListType = lstOtherListType.Where(x => x.functions.Contains(request.Functions));
        }

        if (request.Results != null && request.Results.Length > 0)
        {
            lstOtherListType = lstOtherListType.Where(x => x.result_logging.Contains(request.Results));
        }

        // Đếm số lượng
        int countElements = lstOtherListType.Count();

        // Số lượng trang
        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        // Data Sau phân trang
        var dataList = await lstOtherListType.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();

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

    public async Task<APIResponse> GetListCallApi(FilterLoggingRequest request)
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
        //.Take(request.PageSize).Skip(skipElements)
        // Khai báo mảng ban đầu
        var lstOtherListType = (from p in _context.Loggings
                                where p.is_call_api == true
                                orderby p.date_created descending
                                select new
                                {
                                    user_created = p.user_created,
                                    application = p.application,
                                    actions = p.actions,
                                    content = p.content,
                                    IP = p.IP,
                                    api_name = p.api_name,
                                    is_login = p.is_login,
                                    date_created = p.date_created,
                                    result_logging = p.result_logging
                                });

        // Nếu tồn tại Where theo tên
        if (request.Search != null && request.Search.Length > 0)
        {
            lstOtherListType = lstOtherListType.Where(x => x.user_created.Contains(request.Search) || x.actions.Contains(request.Search));
        }

        if (request.Applications != null && request.Applications.Length > 0)
        {
            lstOtherListType = lstOtherListType.Where(x => x.application.Contains(request.Applications));
        }

        // Đếm số lượng
        int countElements = lstOtherListType.Count();

        // Số lượng trang
        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        // Data Sau phân trang
        var dataList = await lstOtherListType.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();
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

    public async Task<APIResponse> GetListLogIn(FilterLoggingRequest request)
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
        int skipElements = (int)((request.PageNo - 1) * request.PageSize);
        //.Take(request.PageSize).Skip(skipElements)
        // Khai báo mảng ban đầu
        var lstOtherListType = (from p in _context.Loggings
                                where p.is_login == true
                                orderby p.date_created descending
                                select new
                                {
                                    user_created = p.user_created,
                                    application = p.application,
                                    actions = p.actions,
                                    IP = p.IP,
                                    is_login = p.is_login,
                                    date_created = p.date_created,
                                    result_logging = p.result_logging
                                });

        // Nếu tồn tại Where theo tên
        if (request.Search != null && request.Search.Length > 0)
        {
            lstOtherListType = lstOtherListType.Where(x => x.user_created.Contains(request.Search) || x.actions.Contains(request.Search));
        }

        if (request.Applications != null && request.Applications.Length > 0)
        {
            lstOtherListType = lstOtherListType.Where(x => x.application.Contains(request.Applications));
        }

        // Đếm số lượng
        int countElements = lstOtherListType.Count();

        // Số lượng trang
        int totalPage = countElements > 0
                ? (int)Math.Ceiling(countElements / (double)request.PageSize)
                : 0;

        // Data Sau phân trang
        var dataList = await lstOtherListType.Take(request.PageSize * request.PageNo).Skip(skipElements).ToListAsync();
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
