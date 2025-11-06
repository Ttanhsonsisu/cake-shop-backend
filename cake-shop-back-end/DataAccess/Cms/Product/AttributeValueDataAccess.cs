using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Product;
using cake_shop_back_end.Models.CakeProduct;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Product;

public class AttributeValueDataAccess(AppDbContext _context) : IAttributeValue
{
    public async Task<APIResponse> ChangeStatusAsync(AttributeValueRequest req, string username)
    {
        if(req.Id == null)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_VALUE_NOT_FOUND");
        }

        // CHECK EXISTS 
        var data = await _context.AttributeValues.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_VALUE_NOT_FOUND");
        }

        try
        {
            // code change here
        }
        catch (Exception)
        {

            return new APIResponse("ERROR_CHANGE_STATUS_ATTRIBUTE_VALUE_FAIL");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateAsync(AttributeValueRequest request, string username)
    {
        if (string.IsNullOrEmpty(request.Value))
        {
            return new APIResponse("ERROR_VALUE_ATTRIBUTE_VALUE_REQUIRED");
        }

        if (request.AttributeId == null || request.AttributeId == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_NOT_FOUND");
        }

        var attributeExists = _context.Attributes.Any(a => a.id == request.AttributeId);

        if (!attributeExists)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_NOT_FOUND");
        }

        var dataInsert = new AttributeValue();
        dataInsert.id = Guid.NewGuid();
        dataInsert.attribute_id = request.AttributeId.Value;
        dataInsert.value = request.Value.Trim();

        dataInsert.date_updated = DateTime.Now;
        dataInsert.date_created = DateTime.Now;
        dataInsert.user_created = username;
        dataInsert.user_updated = username;

        try
        {
            _context.AttributeValues.Add(dataInsert);
            _context.SaveChanges();
        }
        catch (Exception)
        {
            return new APIResponse("SERVER_INTERNAL_ERROR");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(AttributeValueRequest req, string username)
    {
        if(req.Id == null)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_VALUE_NOT_FOUND");
        }

        // CHECK EXISTS
        var data = await _context.AttributeValues.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_VALUE_NOT_FOUND");
        }

        try
        {
            _context.AttributeValues.Remove(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {

            return new APIResponse("SERVER_INTERNAL_ERROR");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_VALUE_NOT_FOUND");
        }

        var data = await _context.AttributeValues.FindAsync(id);

        var response = new APIResponse(200)
        {
           Data = data
        };

        return response;
    }

    public async Task<APIResponse> GetListAsync(AttributeValueRequest request)
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

        var query = _context.AttributeValues.AsQueryable();

        if (request.Name != null && request.Name.Length > 0)
        {
            string keywordPattern = $"%{request.Name}%";
            query = query.Where(x =>
                (x.value != null && EF.Functions.Like(x.value, keywordPattern))
            );
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
                d.attribute_id,
                attribute = _context.Attributes
                    .Where(a => a.id == d.attribute_id)
                    .Select(a => new
                    {
                        a.id,
                        a.name
                    })
                    .FirstOrDefault(), // get attribute info < name >
                d.value
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

    public async Task<APIResponse> UpdateAsync(AttributeValueRequest request, string username)
    {
        if (string.IsNullOrEmpty(request.Value))
        {
            return new APIResponse("ERROR_VALUE_ATTRIBUTE_VALUE_REQUIRED");
        }

        if (request.AttributeId == null || request.AttributeId == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_NOT_FOUND");
        }
        if (request.Id == null)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_VALUE_NOT_FOUND");
        }

        var attributeExists = _context.Attributes.Any(a => a.id == request.AttributeId);
        if (!attributeExists)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_NOT_FOUND");
        }
        var attributeValueData = await _context.AttributeValues.FindAsync(request.Id);
        
        if (attributeValueData == null)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_VALUE_NOT_FOUND");
        }

        attributeValueData.attribute_id = request.AttributeId.Value;
        attributeValueData.value = request.Value.Trim();

        attributeValueData.date_updated = DateTime.Now;
        attributeValueData.user_updated = username;

        try
        {
            _context.AttributeValues.Update(attributeValueData);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("SERVER_INTERNAL_ERROR");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetListInAttribute(AttributeValueRequest request)
    {
        if (request.AttributeId == null || request.AttributeId == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_NOT_FOUND");
        }

        if (request.PageSize < 1)
        {
            request.PageSize = Consts.PAGE_SIZE;
        }
        if (request.PageNo < 1)
        {
            request.PageNo = 1;
        }

        var attributeExists = await _context.Attributes.FindAsync(request.AttributeId);
        if (attributeExists == null)
        {
            return new APIResponse("ERROR_ID_ATTRIBUTE_NOT_FOUND");
        }

        int skipElement = (request.PageNo - 1) * request.PageSize;

        var query = _context.AttributeValues.AsQueryable();

        if (request.Name != null && request.Name.Length > 0)
        {
            string keywordPattern = $"%{request.Name}%";
            query = query.Where(x =>
                (x.value != null && EF.Functions.Like(x.value, keywordPattern))
            );
        }

        int countElements = await query.CountAsync();

        int totalPage = countElements > 0
            ? (int)Math.Ceiling(countElements / (double)request.PageSize)
            : 0;

        var data = await query
            .Where(x => x.attribute_id == request.AttributeId)
            //.OrderBy(x => x.orders)
            //.ThenBy(x => x.name)
            .Skip(skipElement)
            .Take(request.PageSize)
            .Select(
            d => new
            {
                d.id,
                d.attribute_id,
                attribute_name = attributeExists.name,
                d.value
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
}
