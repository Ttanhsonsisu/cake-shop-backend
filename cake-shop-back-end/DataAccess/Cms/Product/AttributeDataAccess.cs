using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Product;
using cake_shop_back_end.Models.CakeProduct;
using Microsoft.EntityFrameworkCore;
using Attribute = cake_shop_back_end.Models.CakeProduct.Attribute;

namespace cake_shop_back_end.DataAccess.Cms.Product;

public class AttributeDataAccess(AppDbContext _context) : IAttribute
{
    public async Task<APIResponse> CreateAsync(AttributeRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new APIResponse("ERROR_NAME_REQUIRED");
        }
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return new APIResponse("ERROR_CODE_REQUIRED");
        }
        if (request.AttributeValues?.Count < 1)
        {
            return new APIResponse("ERROR_ATTRIBUTE_VALUE_MISSING");
        }
        // check exists (code)
        var codeDuplicate = await _context.Attributes.AnyAsync(e => e.code == request.Code);
        if (codeDuplicate)
        {
            return new APIResponse("ERROR_CODE_EXISTS");
        }

        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // insert attribute 
            var attributeInsert = new Attribute();
            attributeInsert.id = Guid.NewGuid();
            attributeInsert.status = (int)request.Status;
            attributeInsert.name = request.Name;
            attributeInsert.code = request.Code;
            attributeInsert.orders = request.Orders;
            attributeInsert.allow_multiple_values = request.AllowMultipleValues;
            attributeInsert.description = request.Description;

            attributeInsert.user_created = username;
            attributeInsert.date_created = DateTime.Now;
            attributeInsert.user_updated = username;
            attributeInsert.date_updated = DateTime.Now;

            // insert attribute valuse 
            var listAttributeValueInsert = new List<AttributeValue>();
            foreach (var item in request.AttributeValues)
            {
                if (item == null || item.value == null) continue;

                var itemAdd = new AttributeValue();
                itemAdd.id = Guid.NewGuid();
                itemAdd.value = item.value;
                itemAdd.attribute_id = attributeInsert.id;

                listAttributeValueInsert.Add(itemAdd);
            }

            
            if (listAttributeValueInsert.Count < 1)
            {
                return new APIResponse("ERROR_NOT_VALUE_INSERT");
            }

            await _context.Attributes.AddAsync(attributeInsert);
            await _context.AttributeValues.AddRangeAsync(listAttributeValueInsert);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            await transaction.CommitAsync().ConfigureAwait(false);
            await transaction.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync().ConfigureAwait(false);
            await transaction.DisposeAsync().ConfigureAwait(false);

            return new APIResponse("ERROR_INTERNAL_SERVER");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> UpdateAsync(AttributeRequest request, string username)
    {
        if (request == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new APIResponse("ERROR_NAME_MISSING");
        }
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return new APIResponse("ERROR_CODE_MISSING");
        }
        
        var existingAttribute = await _context.Attributes.FindAsync(request.Id);
        var attributeValue = await _context.AttributeValues.Where(e => e.attribute_id == request.Id).ToListAsync();
        if (existingAttribute == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        if (existingAttribute.code != request.Code &&
            await _context.Attributes.AnyAsync(a => a.code == request.Code && a.id != request.Id))
        {
            return new APIResponse("ERROR_CODE_EXISTS");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            existingAttribute.code = request.Code;
            existingAttribute.status = request.Status;
            existingAttribute.orders = request.Orders;
            existingAttribute.name = request.Name;
            existingAttribute.date_updated = DateTime.Now;
            existingAttribute.user_updated = username;

            var requestedValueIds = request.AttributeValues
            .Where(v => v.id != Guid.Empty)
            .Select(v => v.id)
            .ToHashSet(); 

            var valuesToDelete = attributeValue
                .Where(v => !requestedValueIds.Contains(v.id))
                .ToList();

            if (valuesToDelete.Any())
            {
                _context.AttributeValues.RemoveRange(valuesToDelete);
            }

            foreach (var item in request.AttributeValues)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.value)) continue;

                if (item.id != Guid.Empty)
                {
                    var valueToUpdate = attributeValue
                                            .FirstOrDefault(v => v.id == item.id);

                    if (valueToUpdate != null)
                    {
                        valueToUpdate.value = item.value;
                    }
                }
                else
                {
                    var valueToAdd = new AttributeValue
                    {
                        id = Guid.NewGuid(), 
                        value = item.value,
                        attribute_id = existingAttribute.id 
                    };
                    await _context.AttributeValues.AddAsync(valueToAdd);
                }
            }

            int finalCount = attributeValue.Count
                             - valuesToDelete.Count
                             + request.AttributeValues.Count();

            if (finalCount < 1)
            {
                return new APIResponse("ERROR_NO_DATA_INSERT");
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            await transaction.CommitAsync().ConfigureAwait(false);

            return new APIResponse(200); 
        }
        catch (Exception ex)
        {
            return new APIResponse("INTERNAL_SERVER_ERROR");
        }
    }

   
    public async Task<APIResponse> DeleteAsync(AttributeRequest req, string username)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_NULL");
        }
        if (req.Id == null || req.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_MISSING_ID");
        }

        var data = await _context.Attributes.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            var childValues = await _context.AttributeValues
                .Where(x => x.attribute_id == data.id)
                .ToListAsync();

            if (childValues.Any())
            {
                _context.AttributeValues.RemoveRange(childValues);
            }

            _context.Attributes.Remove(data);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERORR " + ex.Message.ToString().ToUpper());
        }

        return new APIResponse(200);
    }

    // Dùng AttributeRequest
    public async Task<APIResponse> ChangeStatusAsync(AttributeRequest req, string username)
    {
        if (req == null)
        {
            return new APIResponse("ERROR_REQUEST_IS_NULL");
        }
        if (req.Id == null || req.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING");
        }
        if (req.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        var data = await _context.Attributes.FindAsync(req.Id);
        if (data == null)
        {
            return new APIResponse("ERROR_ID_NOT_EXISTS");
        }

        try
        {
            data.status = (int)req.Status;
            data.user_updated = username;
            data.date_updated = DateTime.Now;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_UPDATE " + ex.Message.ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return new APIResponse("ID_NOT_INVALID");
        }

        var attribute = await _context.Attributes.FindAsync(id);
        var attributeValue = await _context.AttributeValues.Where(e => e.attribute_id == id).ToListAsync();

        var dataResult = new
        {
            attribute,
            attributeValue,
        };

        return new APIResponse(dataResult);
        
    }

    public async Task<APIResponse> GetListAsync(AttributeRequest request)
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

        var query = _context.Attributes.AsQueryable();

        if (request.Name != null && request.Name.Length > 0)
        {
            string keywordPattern = $"%{request.Name}%";
            query = query.Where(x =>
                (x.name != null && EF.Functions.Like(x.name, keywordPattern)) ||
                (x.code != null && EF.Functions.Like(x.code, keywordPattern))
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
            .OrderBy(x => x.orders)
            .ThenBy(x => x.name)
            .Skip(skipElement)
            .Take(request.PageSize)
            .Select(
            d => new
            {
                d.id,
                d.code,
                d.name,
                d.description,
                d.status,
                d.orders,
                attributeValues = _context.AttributeValues.Where(e => e.attribute_id == d.id).Select(v => new { id = v.id, value = v.value })
                .ToList()
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
