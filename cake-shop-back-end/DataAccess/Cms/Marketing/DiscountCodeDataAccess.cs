using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Marketing;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Interfaces.Cms.Marketing;
using cake_shop_back_end.Models.Discount;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Marketing;

public class DiscountCodeDataAccess(AppDbContext _context) : IDiscountCode
{
    public async Task<APIResponse> GetListAsync(DiscountCodeRequest request)
    {
        if (request.PageSize < 1) request.PageSize = Consts.PAGE_SIZE;
        if (request.PageNo < 1) request.PageNo = 1;

        var query = _context.Set<DiscountCode>().AsQueryable();

        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(x => x.name.ToLower().Contains(request.Name.ToLower()));
        }
        if (request.IsActive != null)
        {
            query = query.Where(x => x.is_active == request.IsActive);
        }
        if (request.StartDate != null)
        {
            query = query.Where(x => x.end_date >= request.StartDate);
        }

        int totalRecord = await query.CountAsync();
        int totalPage = (int)Math.Ceiling(totalRecord / (double)request.PageSize);

        var data = await query
            .OrderByDescending(x => x.date_created)
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var response = new DataListResponse
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalPage = totalPage,
            Data = data
        };

        return new APIResponse(response);
    }

    public async Task<APIResponse> ChangeStatusAsync(DiscountCodeRequest request, string username)
    {
        if (request.Id == null) return new APIResponse("ERROR_INVALID_REQUEST");

        if (request.IsActive == null && request.Status == null)
        {
            return new APIResponse("ERROR_NO_STATUS_PROVIDED");
        }

        var entity = await _context.Set<DiscountCode>().FindAsync(request.Id);
        if (entity == null) return new APIResponse("ERROR_NOT_FOUND");

        try
        {
            //entity.is_active = request.IsActive.Value;
            if (request.IsActive != null) entity.is_active = request.IsActive.Value;
            if (request.Status != null) entity.status = request.Status;

            entity.user_updated = username;
            entity.date_updated = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_" + ex.Message.ToUpper());
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        var code = await _context.Set<DiscountCode>().FindAsync(id);
        if (code == null)
        {
            return new APIResponse("ERROR_NOT_FOUND");
        }

        var targets = await _context.Set<DiscountCodeTarget>()
            .Where(x => x.campaign_id == id)
            .ToListAsync();

        var result = new
        {
            Info = code,
            Targets = targets
        };

        return new APIResponse(result) { Code = "200" };
    }

    public async Task<APIResponse> CreateAsync(DiscountCodeRequest request, string username)
    {
        if (request == null) return new APIResponse("ERROR_REQUEST_NULL");
        if (string.IsNullOrEmpty(request.Name)) return new APIResponse("ERROR_NAME_REQUIRED");
        if (request.StartDate == null || request.EndDate == null) return new APIResponse("ERROR_DATE_REQUIRED");

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newId = Guid.NewGuid(); 

                var entity = new DiscountCode
                {
                    id = newId,
                    name = request.Name,
                    description = request.Description,
                    start_date = request.StartDate.Value,
                    end_date = request.EndDate.Value,
                    is_active = request.IsActive ?? true,
                    discount_type = request.DiscountType ?? "percent",
                    discount_value = request.DiscountValue ?? 0,
                    max_discount = request.MaxDiscount,
                    user_created = username,
                    date_created = DateTime.Now,
                    user_updated = username,
                    date_updated = DateTime.Now
                };

                await _context.Set<DiscountCode>().AddAsync(entity);

                // 2. Tạo Targets (Detail)
                if (request.Targets != null && request.Targets.Any())
                {
                    var listTargets = request.Targets.Select(t => new DiscountCodeTarget
                    {
                        id = Guid.NewGuid(),
                        campaign_id = newId,

                        product_id = t.ProductId,
                        variant_id = t.VariantId
                    }).ToList();

                    await _context.Set<DiscountCodeTarget>().AddRangeAsync(listTargets);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new APIResponse(200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new APIResponse("ERROR_" + ex.Message.ToUpper());
            }
        });
    }

    public async Task<APIResponse> UpdateAsync(DiscountCodeRequest request, string username)
    {
        if (request.Id == null) return new APIResponse("ERROR_ID_MISSING");

        var entity = await _context.Set<DiscountCode>().FindAsync(request.Id);
        if (entity == null) return new APIResponse("ERROR_NOT_FOUND");

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update thông tin chính
                entity.name = request.Name ?? entity.name;
                entity.description = request.Description;
                if (request.StartDate.HasValue) entity.start_date = request.StartDate.Value;
                if (request.EndDate.HasValue) entity.end_date = request.EndDate.Value;
                if (request.IsActive.HasValue) entity.is_active = request.IsActive.Value;
                if (!string.IsNullOrEmpty(request.DiscountType)) entity.discount_type = request.DiscountType;
                if (request.DiscountValue.HasValue) entity.discount_value = request.DiscountValue.Value;
                entity.max_discount = request.MaxDiscount;
                if (request.Status.HasValue) entity.status = request.Status.Value;

                entity.user_updated = username;
                entity.date_updated = DateTime.Now;

                // Update Targets
                if (request.Targets != null)
                {
                    // Xóa các target cũ dựa trên cột campaign_id (là ID của Code)
                    var oldTargets = _context.Set<DiscountCodeTarget>()
                                             .Where(x => x.campaign_id == request.Id);
                    _context.Set<DiscountCodeTarget>().RemoveRange(oldTargets);

                    // Thêm target mới
                    var newTargets = request.Targets.Select(t => new DiscountCodeTarget
                    {
                        id = Guid.NewGuid(),

                        // Map ID của Code đang sửa vào cột campaign_id
                        campaign_id = request.Id.Value,

                        product_id = t.ProductId,
                        variant_id = t.VariantId
                    }).ToList();

                    await _context.Set<DiscountCodeTarget>().AddRangeAsync(newTargets);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new APIResponse(200, "Updated successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new APIResponse("ERROR_" + ex.Message.ToUpper());
            }
        });
    }

    public async Task<APIResponse> DeleteAsync(DiscountCodeRequest request, string username)
    {
        if (request.Id == null) return new APIResponse("ERROR_ID_MISSING");

        var entity = await _context.Set<DiscountCode>().FindAsync(request.Id);
        if (entity == null) return new APIResponse("ERROR_NOT_FOUND");

        try
        {
            // Xóa targets trước
            var targets = _context.Set<DiscountCodeTarget>().Where(x => x.campaign_id == request.Id);
            _context.Set<DiscountCodeTarget>().RemoveRange(targets);

            // Xóa Code
            _context.Set<DiscountCode>().Remove(entity);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_" + ex.Message.ToUpper());
        }

        return new APIResponse(200);
    }
}
