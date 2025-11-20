using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Marketing;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Interfaces.Cms.Marketing;
using cake_shop_back_end.Models.Discount;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Marketing;

public class CampaignDataAccess(AppDbContext _context) : ICampaign
{
    public async Task<APIResponse> GetListAsync(CampaignRequest request)
    {
        if (request.PageSize < 1) request.PageSize = 10;
        if (request.PageNo < 1) request.PageNo = 1;

        var query = _context.DiscountCampaigns.AsQueryable();

        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(x => x.name.ToLower().Contains(request.Name.ToLower()));
        }
        if (request.IsActive != null)
        {
            query = query.Where(x => x.is_active == request.IsActive);
        }
        // Filter theo thời gian (VD: lấy các camapgin đang chạy trong khoảng request)
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

    public async Task<APIResponse> GetDetailAsync(Guid id)
    {
        var campaign = await _context.Set<DiscountCampaign>().FindAsync(id);
        if (campaign == null)
        {
            return new APIResponse("ERROR_NOT_FOUND");
        }

        // Lấy thêm danh sách targets
        var targets = await _context.DiscountTargets
            .Where(x => x.campaign_id == id)
            .ToListAsync();

        var result = new
        {
            Info = campaign,
            Targets = targets
        };

        return new APIResponse(result) { Code = "200" };
    }

    public async Task<APIResponse> CreateAsync(CampaignRequest request, string username)
    {
        if (request == null) return new APIResponse("ERROR_REQUEST_NULL");
        if (string.IsNullOrEmpty(request.Name)) return new APIResponse("ERROR_NAME_REQUIRED");
        if (request.StartDate == null || request.EndDate == null) return new APIResponse("ERROR_DATE_REQUIRED");
        if (request.StartDate > request.EndDate) return new APIResponse("ERROR_DATE_INVALID");

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newId = Guid.NewGuid();
                var entity = new DiscountCampaign
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

                await _context.DiscountCampaigns.AddAsync(entity);

                // Thêm Targets (nếu có)
                if (request.Targets != null && request.Targets.Any())
                {
                    var listTargets = request.Targets.Select(t => new DiscountTarget
                    {
                        id = Guid.NewGuid(),
                        campaign_id = newId,
                        product_id = t.ProductId,
                        variant_id = t.VariantId
                    }).ToList();

                    await _context.DiscountTargets.AddRangeAsync(listTargets);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new APIResponse(200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new APIResponse("INTERNAL_SERVER_ERROR");
            }
        });
    }

    public async Task<APIResponse> UpdateAsync(CampaignRequest request, string username)
    {
        if (request.Id == null) return new APIResponse("ERROR_ID_MISSING");

        var entity = await _context.Set<DiscountCampaign>().FindAsync(request.Id);
        if (entity == null) return new APIResponse("ERROR_NOT_FOUND");

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update Info
                entity.name = request.Name ?? entity.name;
                entity.description = request.Description;
                if (request.StartDate.HasValue) entity.start_date = request.StartDate.Value;
                if (request.EndDate.HasValue) entity.end_date = request.EndDate.Value;
                if (request.IsActive.HasValue) entity.is_active = request.IsActive.Value;
                if (!string.IsNullOrEmpty(request.DiscountType)) entity.discount_type = request.DiscountType;
                if (request.DiscountValue.HasValue) entity.discount_value = request.DiscountValue.Value;
                entity.max_discount = request.MaxDiscount;

                entity.user_updated = username;
                entity.date_updated = DateTime.Now;

                // Update Targets: Xóa cũ -> Thêm mới (Cách đơn giản nhất)
                if (request.Targets != null)
                {
                    var oldTargets = _context.DiscountTargets.Where(x => x.campaign_id == request.Id);
                    _context.DiscountTargets.RemoveRange(oldTargets);

                    var newTargets = request.Targets.Select(t => new DiscountTarget
                    {
                        id = Guid.NewGuid(),
                        campaign_id = request.Id.Value,
                        product_id = t.ProductId,
                        variant_id = t.VariantId
                    }).ToList();

                    await _context.DiscountTargets.AddRangeAsync(newTargets);
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

    public async Task<APIResponse> ChangeStatusAsync(CampaignRequest request, string username)
    {
        if (request.Id == null) return new APIResponse("ERROR_INVALID_REQUEST");

        if (request.IsActive == null && request.Status == null)
        {
            return new APIResponse("ERROR_STATUS_MISSING");
        }

        var entity = await _context.Set<DiscountCampaign>().FindAsync(request.Id);
        if (entity == null) return new APIResponse("ERROR_NOT_FOUND");

        
        try
        {
            if (request.IsActive != null)
            {
                entity.is_active = request.IsActive.Value;
            }

            if (request.Status != null)
            { 
                entity.status = request.Status;
            }

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

    public async Task<APIResponse> DeleteAsync(CampaignRequest request, string username)
    {
        if (request.Id == null) return new APIResponse("ERROR_ID_MISSING");

        var entity = await _context.Set<DiscountCampaign>().FindAsync(request.Id);
        if (entity == null) return new APIResponse("ERROR_NOT_FOUND");

        try
        {
            // Xóa Targets trước (Nếu Database không set Cascade Delete)
            var targets = _context.DiscountTargets.Where(x => x.campaign_id == request.Id);
            _context.DiscountTargets.RemoveRange(targets);

            // Xóa Campaign
            _context.DiscountCampaigns.Remove(entity);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_" + ex.Message.ToUpper());
        }

        return new APIResponse(200);
    }
}
