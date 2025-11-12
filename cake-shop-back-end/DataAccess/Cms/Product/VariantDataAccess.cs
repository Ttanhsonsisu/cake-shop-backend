using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
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
            newVariant.product_id = request.ProductId;
            newVariant.sku = request.Sku;
            newVariant.price = request.Price;

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

    public Task<APIResponse> GetDetailAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<APIResponse> GetListAsync(VariantRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<APIResponse> PublishProductAsync(VariantRequest request, string username)
    {
        throw new NotImplementedException();
    }

    public Task<APIResponse> UpdateAsync(VariantRequest request, string username)
    {
        throw new NotImplementedException();
    }
}
