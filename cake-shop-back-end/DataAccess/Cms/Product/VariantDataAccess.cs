using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Product;
using cake_shop_back_end.Models.CakeProduct;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Product;

public class VariantDataAccess(AppDbContext _context) : IVariant
{
    public async Task<APIResponse> ChangeStatusAsync(VariantRequest request, string username)
    {
        // validate request
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_INVALID_REQUEST");
        }

        var dataChange = await _context.Variants.FindAsync(request.Id);
        if (dataChange == null)
        {
            return new APIResponse("ERROR_NOT_FOUND");
        }
        try
        {
            // change status
            // code here
            //dataChange
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_SYSTEM_EXCEPTION");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateDraftAsync(VariantRequest request, string username)
    {
        // validate request
        if (request.ProductId == Guid.Empty || string.IsNullOrEmpty(request.Sku) || request.Price <= 0)
        {
            return new APIResponse("ERROR_INVALID_REQUEST");
        }
        if (string.IsNullOrEmpty(request.Sku))
        {
            return new APIResponse("ERROR_INVALID_REQUEST");
        }
        if (request.Price <= 0)
        {
            return new APIResponse("ERROR_INVALID_REQUEST");
        }
        // check exists product id
        var dataExists = await _context.ProductCakes.FindAsync(request.ProductId);
        if (dataExists == null)
        {
            return new APIResponse("ERROR_PRODUCT_NOT_EXISTS");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // create new variant
            var newVariant = new Variant();
            newVariant.id = Guid.NewGuid();
            newVariant.product_id = (Guid)request.ProductId;
            newVariant.sku = (string)request.Sku;
            newVariant.price = (decimal)request.Price;

            newVariant.date_updated = DateTime.Now;
            newVariant.user_updated = username;
            newVariant.date_created = DateTime.Now;
            newVariant.user_created = username;

            await _context.Variants.AddAsync(newVariant);
            // create attribute in variant

            var variantAttributeValue = new List<VariantAttributeValue>();

            foreach (var attr in request.VariantAtributeValues)
            {
                // validate attribute id and value id
                var newVariantAttr = new VariantAttributeValue();
                newVariantAttr.id = Guid.NewGuid();
                newVariantAttr.attribute_id = attr.AttributeId;
                newVariantAttr.attribute_value_id = attr.AttributeValueId;
                newVariantAttr.variant_id = newVariant.id;
                newVariantAttr.date_created = DateTime.Now;
                newVariantAttr.user_created = username;
                newVariantAttr.date_updated = DateTime.Now;
                newVariantAttr.user_updated = username;
                variantAttributeValue.Add(newVariantAttr);
            }

            if (variantAttributeValue.Count < 1)
            {
                await transaction.RollbackAsync();
                return new APIResponse("ERROR_INVALID_REQUEST");
            }

            await _context.VariantAttributeValues.AddRangeAsync(variantAttributeValue);

            // update count variant in product
            dataExists.variants = dataExists.variants != null ? dataExists.variants + 1 : 1;
            _context.ProductCakes.Update(dataExists);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new APIResponse("ERROR_SYSTEM_EXCEPTION");
        }
        return new APIResponse(200);
    }

    public async Task<APIResponse> DeleteAsync(VariantRequest request, string username)
    {
        // validate request
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_INVALID_REQUEST");
        }
        var dataDelete = await _context.Variants.FindAsync(request.Id);
        if (dataDelete == null)
        {
            return new APIResponse("ERROR_NOT_FOUND");
        }
        try
        {
            _context.Variants.Remove(dataDelete);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_SYSTEM_EXCEPTION");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        // validate request 
        if (id == Guid.Empty)
        {
            return new APIResponse("ERROR_INVALID_REQUEST");
        }

        var data = await (from v in _context.Variants
                          where v.id == id 
                          join p in _context.ProductCakes on v.product_id equals p.id into ps
                          from p in ps.DefaultIfEmpty()
                          select new 
                          {
                              id = v.id,
                              sku = v.sku,
                              price = v.price,
                              product_id = v.product_id,
                              product_name = p == null ? "" : p.name,
                              date_created = v.date_created,
                              date_updated = v.date_updated,
                              attributes = (
                                  from vav in _context.VariantAttributeValues
                                  where vav.variant_id == v.id 
                                  join a in _context.Attributes on vav.attribute_id equals a.id
                                  join av in _context.AttributeValues on vav.attribute_value_id equals av.id
                                  group new { a, av } by new { a.id, a.name, a.code } into g
                                  select new 
                                  {
                                      attribute_id = g.Key.id,
                                      attribute_name = g.Key.name,
                                      attribute_code = g.Key.code,
                                      values = g.Select(x => new 
                                      {
                                          value_id = x.av.id,
                                          value = x.av.value
                                      }).ToList()
                                  }
                              ).ToList() 

                          }).FirstOrDefaultAsync(); 

        if (data == null)
        {
            return new APIResponse("ERROR_NOT_FOUND");
        }

        return new APIResponse(data);
    }

    public async Task<APIResponse> GetListAsync(VariantRequest request)
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

        int skipElements = (request.PageNo - 1) * request.PageSize;

        // Base query: variants with product name
        var query = from v in _context.Variants
                    join p in _context.ProductCakes on v.product_id equals p.id into ps
                    from p in ps.DefaultIfEmpty()
                    orderby v.date_created descending
                    select new
                    {
                        id = v.id,
                        sku = v.sku,
                        price = v.price,
                        product_id = v.product_id,
                        product_name = p == null ? "" : p.name,
                        date_created = v.date_created,
                        date_updated = v.date_updated
                    };

        // Filters
        if (request.Id != null && request.Id != Guid.Empty)
        {
            query = query.Where(x => x.id == request.Id);
        }

        if (request.ProductId != Guid.Empty && request.ProductId != null)
        {
            query = query.Where(x => x.product_id == request.ProductId);
        }

        if (!string.IsNullOrEmpty(request.Sku))
        {
            var pattern = $"%{request.Sku}%";
            query = query.Where(x => EF.Functions.Like(x.sku, pattern));
        }

        if (request.Price > 0)
        {
            query = query.Where(x => x.price == request.Price);
        }

        // Count & paging
        int countElements = await query.CountAsync();

        int totalPage = countElements > 0
            ? (int)Math.Ceiling(countElements / (double)request.PageSize)
            : 0;

        var dataList = await query
            .Skip(skipElements)
            .Take(request.PageSize)
            .ToListAsync();

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

    public Task<APIResponse> UpdateAsync(VariantRequest request, string username)
    {
        throw new NotImplementedException();
    }

    public async Task<APIResponse> AddImageAsync(ProductImageRequest request, string username)
    {
        // validate request
        if (request.VariantId == null || request.VariantId == Guid.Empty || string.IsNullOrEmpty(request.ImageUrl))
        {
            return new APIResponse("ERROR_INVALID_REQUEST");
        }
        // check exists product id
        var dataExists = await _context.Variants.FindAsync(request.VariantId);
        if (dataExists == null) {
            return new APIResponse("ERROR_PRODUCT_NOT_EXISTS");
        }
        // save image
        var data = new VariantImage();
        data.variant_id = request.VariantId;
        data.image_url = request.ImageUrl;
        data.date_created = DateTime.Now;
        data.user_created = username;

        try
        {
            await _context.VariantImages.AddAsync(data);
            await  _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("ERROR_SYSTEM_EXCEPTION");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> RemoveImageAsync(ProductImageRequest request, string username)
    {
        // validate request
        if (request.Id == null || request.Id < 0)
        {
            return new APIResponse("ERROR_INVALID_REQUEST");
        }
        var dataDelete = await _context.VariantImages.FindAsync(request.Id);
        if (dataDelete == null)
        {
            return new APIResponse("ERROR_NOT_FOUND");
        }
        try
        {
            _context.VariantImages.Remove(dataDelete);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_SYSTEM_EXCEPTION");
        }
        return new APIResponse(200);
    }
}
