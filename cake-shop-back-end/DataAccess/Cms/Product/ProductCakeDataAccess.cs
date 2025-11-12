using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Product;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Product;
using cake_shop_back_end.Models.CakeProduct;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Product;

public class ProductCakeDataAccess(AppDbContext _context) : IProductCake
{
    public async Task<APIResponse> ChangeStatusAsync(ProductCakeRequest request, string username)
    {

        // code change status here
        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateDraftAsync(ProductCakeRequest request, string username)
    {
        // validate request here
        if (string.IsNullOrEmpty(request.Name))
        {
            return new APIResponse("Product name is required");
        }
        if(request.BasePrice == null || request.BasePrice <= 0)
        {
            return new APIResponse("Base price must be greater than 0");
        }
        if(request.Categories == null || !request.Categories.Any())
        {
            return new APIResponse("At least one category is required");
        }
        // code create draft here
        var dataDraft = new ProductCake();
        dataDraft.id = Guid.NewGuid();
        dataDraft.name = request.Name;
        dataDraft.description = request.Description;
        dataDraft.base_price = request.BasePrice;
        dataDraft.storage = request.Storage;
        dataDraft.status = request.Status ?? 0; 
        dataDraft.is_visible = request.IsVisible ?? true; 
        dataDraft.variants = request.Variants ?? 0;

        dataDraft.user_created = username;
        dataDraft.date_created = DateTime.UtcNow;

        // insert cake category to database
        var dataCategories = new List<ProductCakeCategory>();
        foreach (var categoryId in request.Categories)
        {
            if (categoryId == Guid.Empty) continue;
            if(dataCategories.Any(dc => dc.category_id == categoryId)) continue;

            dataCategories.Add(new ProductCakeCategory
            {
                id = Guid.NewGuid(),
                productCake_id = dataDraft.id,
                category_id = categoryId
            });
        }

        if (!dataCategories.Any())
        {
            return new APIResponse("At least one valid category is required");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.ProductCakes.AddAsync(dataDraft);
            await _context.ProductCakeCategories.AddRangeAsync(dataCategories);

            await _context.SaveChangesAsync().ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);

            return new APIResponse(200);
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_SERVER_INTERNAL");
        }

    }

    public async Task<APIResponse> DeleteAsync(ProductCakeRequest request, string username)
    {

        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ID_MISSING_OR_INVALID");
        }
        // check exists
        var data = await _context.ProductCakes.FindAsync(request.Id);
        if (data == null)
        {
            return new APIResponse("ID_MISSING_OR_INVALID");
        }
        // find table categories
        var listCategoryProduct = await _context.ProductCakeCategories.Where(e => e.productCake_id == data.id).ToListAsync();
        // find varial

        //
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (listCategoryProduct.Any() && listCategoryProduct.Count() > 0)
            {
                _context.ProductCakeCategories.RemoveRange(listCategoryProduct);
            }

            _context.ProductCakes.Remove(data);

            await _context.SaveChangesAsync().ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);

            return new APIResponse(200);
        }
        catch (Exception ex)
        {
            return new APIResponse("INTERNAL_SERVER_ERROR");
        }
    }

    public Task<APIResponse> GetDetailAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<APIResponse> GetListAsync(ProductCakeRequest request)
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

        var query = _context.ProductCakes.AsQueryable();

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
                d.description,
                d.status,
                d.variants
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

    public async Task<APIResponse> PublishProductAsync(ProductCakeRequest request, string username)
    {
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_MISSING_ID");
        }
        // CHECK EXISTS 
        var data = await _context.ProductCakes.FindAsync(request.Id);
        if (data == null)
        {
            return new APIResponse("ERORR_ID_NOT_EXISTS");
        }

        data.status = 2;

        try
        {
            await _context.SaveChangesAsync();
            return new APIResponse(200);
        }
        catch (Exception)
        {
            return new APIResponse("INTERNAL_SERVER_ERROR");
        }
    }

    public Task<APIResponse> UpdateAsync(ProductCakeRequest request, string username)
    {
        throw new NotImplementedException();
    }
}
