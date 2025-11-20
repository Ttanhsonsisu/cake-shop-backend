using System.Linq;
using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Order;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Interfaces.Cms.Order;
using cake_shop_back_end.Models.Discount;
using cake_shop_back_end.Models.Order;
using Microsoft.EntityFrameworkCore;

namespace cake_shop_back_end.DataAccess.Cms.Order;

public class OrderDataAccess(AppDbContext _context) : IOrder
{
    public async Task<APIResponse> CancelOrderAsync(OrderRequest request, string username)
    {
        // validate input
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING_OR_INVALID");
        }
        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null)
        {
            return new APIResponse("ERROR_ORDER_NOT_FOUND");
        }
        // update order status to cancelled
        order.order_status = -1;
        order.date_updated = DateTime.Now;
        order.user_updated = username;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("SUCCESS_ORDER_CANCELLED");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> ChangeStatusOrderAsync(OrderRequest request, string username)
    {
        // validate input
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING_OR_INVALID");
        }
        if (request.OrderStatus == null)
        {
            return new APIResponse("ERROR_ORDER_STATUS_MISSING_OR_INVALID");
        }

        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null)
        {
            return new APIResponse("ERROR_ORDER_NOT_FOUND");
        }

        try
        {
            order.order_status = request.OrderStatus ?? 1;
            order.date_updated = DateTime.Now;
            order.user_updated = username;

            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("SUCCESS_ORDER_STATUS_UPDATED");
        }

        return new APIResponse(200);

    }

    public async Task<APIResponse> ComfirmOrderAsync(OrderRequest request, string username)
    {
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING_OR_INVALID");
        }
        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null)
        {
            return new APIResponse("ERROR_ORDER_NOT_FOUND");
        }

        bool orderCheckDelete = order.is_delete ?? false;
        if (orderCheckDelete)
        {
            return new APIResponse("ERROR_CANNOT_CONFIRM_DELETED_ORDER");
        }
        try
        {
            order.order_status = 2; // confirmed
            order.date_updated = DateTime.Now;
            order.user_updated = username;

            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("SUCCESS_ORDER_CONFIRMED");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> ComfirmPaymentAsync(OrderRequest request, string username)
    {
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING_OR_INVALID");
        }
        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null)
        {
            return new APIResponse("ERROR_ORDER_NOT_FOUND");
        }

        bool orderIsDelete = order.is_delete ?? false;
        if (orderIsDelete)
        {
            return new APIResponse("ERROR_CANNOT_CONFIRM_PAYMENT_FOR_DELETED_ORDER");
        }
        try
        {
            order.is_paid = true;
            order.date_updated = DateTime.Now;
            order.user_updated = username;
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("SUCCESS_PAYMENT_CONFIRMED");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> CreateOrderDrafFromAdminAsync(OrderRequest request, string username)
    {
        // Sử dụng Transaction để đảm bảo nếu lỗi ở bước nào thì rollback sạch sẽ
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var newOrderId = Guid.NewGuid();
            var now = DateTime.Now;

            // =================================================================================
            // 🚀 BƯỚC 1: LẤY DANH SÁCH ID SẢN PHẨM ĐỂ QUERY TỐI ƯU
            // (Tránh việc load toàn bộ bảng Target ra RAM gây sập server)
            // =================================================================================
            var productIds = request.OrderItems
                .Where(x => x.ProductId.HasValue).Select(x => x.ProductId.Value).Distinct().ToList();
            var variantIds = request.OrderItems
                .Where(x => x.VariantId.HasValue).Select(x => x.VariantId.Value).Distinct().ToList();

            // =================================================================================
            // 🚀 BƯỚC 2: LOAD DỮ LIỆU CẦN THIẾT (PRE-LOAD)
            // =================================================================================

            // 2.1. Load Discount Code và Target của nó (Chỉ load target liên quan đến sản phẩm trong đơn)
            DiscountCode? discountCode = null;
            List<DiscountCodeTarget> relevantCodeTargets = new List<DiscountCodeTarget>();

            if (!string.IsNullOrEmpty(request.DiscountCode))
            {
                discountCode = await _context.DiscountCodes
                    .FirstOrDefaultAsync(x =>
                        x.code == request.DiscountCode &&
                        x.is_active == true &&
                        x.start_date <= now &&
                        x.end_date >= now
                    );

                if (discountCode != null)
                {
                    relevantCodeTargets = await _context.DiscountCodeTargets
                        .Where(x => x.campaign_id == discountCode.id &&
                                   (productIds.Contains(x.product_id.Value) || variantIds.Contains(x.variant_id.Value)))
                        .ToListAsync();
                }
            }

            // 2.2. Load Campaign Active và Target của nó
            var activeCampaigns = await _context.DiscountCampaigns
                .Where(x => x.is_active == true && x.start_date <= now && x.end_date >= now)
                .ToListAsync();

            var activeCampaignIds = activeCampaigns.Select(c => c.id).ToList();

            // Chỉ load những target nào thuộc campaign đang chạy VÀ trùng với sản phẩm khách mua
            var relevantCampaignTargets = await _context.DiscountTargets
                .Where(t => activeCampaignIds.Contains(t.campaign_id) &&
                            (productIds.Contains(t.product_id.Value) || variantIds.Contains(t.variant_id.Value)))
                .ToListAsync();

            // =================================================================================
            // 🚀 BƯỚC 3: TÍNH TOÁN CHI TIẾT TỪNG ITEM (MAIN LOGIC)
            // =================================================================================
            var orderItemsEntities = new List<OrderItem>();
            decimal subtotal = 0;
            decimal totalDiscount = 0;

            foreach (var itemReq in request.OrderItems)
            {
                var quantity = itemReq.Quantity ?? 1;
                // Lưu ý: Ở đây đang tin tưởng giá client gửi lên (Admin nhập). 
                // Nếu là luồng khách mua tự động, bạn nên query lại bảng Product để lấy giá gốc cho an toàn.
                var unitPrice = itemReq.UnitPrice ?? 0;
                var itemTotalRaw = unitPrice * quantity;

                subtotal += itemTotalRaw;
                decimal itemDiscount = 0;

                // --- 3.1. Tính giảm giá từ Campaign (Ưu tiên) ---
                // Lọc nhanh trên RAM từ danh sách đã pre-load
                var itemCampaignTargets = relevantCampaignTargets
                    .Where(t => t.product_id == itemReq.ProductId || t.variant_id == itemReq.VariantId)
                    .ToList();

                foreach (var target in itemCampaignTargets)
                {
                    var campaign = activeCampaigns.FirstOrDefault(c => c.id == target.campaign_id);
                    if (campaign == null) continue;

                    decimal currentCampDiscount = 0;
                    if (campaign.discount_type == "percent")
                    {
                        currentCampDiscount = itemTotalRaw * (campaign.discount_value / 100);
                        if (campaign.max_discount.HasValue)
                        {
                            currentCampDiscount = Math.Min(currentCampDiscount, campaign.max_discount.Value);
                        }
                    }
                    else // Giảm tiền mặt
                    {
                        // Logic: Giảm trên tổng dòng hay giảm trên từng đơn vị sp? 
                        // Thường là giảm 1 lần cho cả dòng item. Nếu giảm trên từng sp thì nhân quantity.
                        currentCampDiscount = campaign.discount_value;
                    }
                    itemDiscount += currentCampDiscount;
                }

                // --- 3.2. Tính giảm giá từ Discount Code (Nếu có) ---
                if (discountCode != null)
                {
                    // Check 1: Code này có target cụ thể nào không? (Nếu list rỗng = áp dụng all, tùy logic db của bạn)
                    // Giả sử: Check trong db, nếu discountCode không có target nào trong bảng target -> Global code
                    // Ở đây tôi dùng logic: Chỉ áp dụng nếu sản phẩm nằm trong target đã load

                    bool isTargeted = relevantCodeTargets.Any(t => t.product_id == itemReq.ProductId || t.variant_id == itemReq.VariantId);

                    // Nếu code áp dụng global hoặc trúng target
                    if (isTargeted)
                    {
                        decimal codeDiscount = 0;
                        if (discountCode.discount_type == "percent")
                        {
                            codeDiscount = itemTotalRaw * (discountCode.discount_value / 100);
                            if (discountCode.max_discount.HasValue)
                                codeDiscount = Math.Min(codeDiscount, discountCode.max_discount.Value);
                        }
                        else
                        {
                            codeDiscount = discountCode.discount_value;
                        }
                        itemDiscount += codeDiscount;
                    }
                }

                // --- 3.3. Finalize Item ---
                // Không cho phép giảm giá vượt quá giá trị món hàng
                if (itemDiscount > itemTotalRaw) itemDiscount = itemTotalRaw;

                totalDiscount += itemDiscount;
                // var finalItemPrice = itemTotalRaw - itemDiscount; 

                var orderItem = new OrderItem
                {
                    id = Guid.NewGuid(),
                    order_id = newOrderId,
                    product_id = itemReq.ProductId,
                    variant_id = itemReq.VariantId,
                    quantity = quantity,
                    price = unitPrice,
                    total_price = itemTotalRaw,
                    // Nếu DB có cột discount/final_price ở item thì uncomment dòng dưới
                    // discount = itemDiscount, 
                    // final_price = finalItemPrice, 

                    date_created = now,
                    user_created = username,
                    date_updated = now,
                    user_updated = username
                };

                orderItemsEntities.Add(orderItem);
            }

            // =================================================================================
            // 🚀 BƯỚC 4: TẠO ORDER VÀ LƯU DATABASE
            // =================================================================================
            decimal deliveryFee = request.DeliveryFee ?? 0;
            decimal totalAmount = subtotal - totalDiscount + deliveryFee;
            // Đảm bảo không âm
            if (totalAmount < 0) totalAmount = 0;

            var orderEntity = new cake_shop_back_end.Models.Order.Order
            {
                id = newOrderId,
                user_id = request.UserId ?? Guid.Empty,

                // Thông tin người nhận
                recipient_name = request.RecipientName,
                recipient_phone = request.RecipientPhone,
                recipient_address = request.RecipientAddress,

                // Thông tin khách vãng lai (nếu có)
                name_guest = request.NameGuest,
                phone_guest = request.PhoneGuest,
                email_guest = request.EmailGuest,

                // Meta data
                delivery_type = request.DeliveryType,
                delivery_time = request.DeliveryTime,
                order_tag = request.OrderTag,
                order_source = request.OrderSource ?? 0, // Nên dùng Enum thay vì số 0

                // Tài chính
                subtotal_amount = subtotal,
                discount_amount = totalDiscount,
                delivery_fee = deliveryFee,
                total_amount = totalAmount,

                // Trạng thái
                payment_method = request.PaymentMethod ?? 0,
                order_status = request.OrderStatus ?? -1, // Nên dùng Enum, ví dụ OrderStatus.Draft
                is_paid = request.IsPaid ?? false,
                is_custom_order = request.IsCustomOrder ?? false,
                is_delete = false,

                date_created = now,
                user_created = username,
                date_updated = now,
                user_updated = username
            };

            // Add vào Context
            await _context.Orders.AddAsync(orderEntity);
            await _context.OrderItems.AddRangeAsync(orderItemsEntities);

            // Commit Transaction
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new APIResponse(200, new { OrderId = newOrderId, Message = "Tạo đơn nháp thành công" });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new APIResponse("ERROR_CREATING_ORDER_DRAFT");
        }
    }
    public async Task<APIResponse> DeleteOrderAsync(OrderRequest request, string username)
    {
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING_OR_INVALID");
        }
        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null)
        {
            return new APIResponse("ERROR_ORDER_NOT_FOUND");
        }
        try
        {
            order.is_delete = true;
            order.date_updated = DateTime.Now;
            order.user_updated = username;
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("SUCCESS_ORDER_DELETED");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> DetailOrderNomalAsync(Guid id)
    {
        try
        {
            var order = await _context.Orders
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.id == id);

            if (order == null)
            {
                return new APIResponse("ERORR_ORDER_NOT_FOUND");

            }

            var itemQuery = from oi in _context.OrderItems
                            join p in _context.ProductCakes on oi.product_id equals p.id
                            join v in _context.Variants on oi.variant_id equals v.id into vGroup
                            from v in vGroup.DefaultIfEmpty()
                            where oi.order_id == id
                            select new
                            {
                                ProductId = p.id,
                                ProductName = p.name,
                                Quantity = oi.quantity,
                                UnitPrice = oi.price,
                                TotalPrice = oi.total_price
                            };

            var items = await itemQuery.ToListAsync();

            // 3. Map dữ liệu sang Response DTO
            var responseData = new
            {
                Id = order.id,
                CreatedAt = order.date_created ?? DateTime.MinValue,

                StatusCode = order.order_status ?? 0,
                Status = order.order_status,

                RecipientName = order.recipient_name,
                RecipientPhone = order.recipient_phone,
                RecipientAddress = order.recipient_address,

                DeliveryFee = order.delivery_fee,
                TotalAmount = order.total_amount,

                // Map Payment Method
                PaymentMethod = order.payment_method,
                IsPaid = order.is_paid,

                Items = items
            };

            return new APIResponse(responseData);
        }
        catch (Exception ex)
        {
            return new APIResponse("ERROR_RETRIEVING_ORDER_DETAILS");
        }
    }

    public async Task<APIResponse> GetOrderListAsync(OrderRequest request)
    {
        if (request.PageSize < 1) request.PageSize = 20;
        if (request.PageNo < 1) request.PageNo = 1;

        var query = _context.Orders.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.RecipientName))
        {
            var pattern = $"%{request.RecipientName}%";
            query = query.Where(x => EF.Functions.Like(x.recipient_name, pattern));
        }

        if (!string.IsNullOrWhiteSpace(request.RecipientPhone))
        {
            var pattern = $"%{request.RecipientPhone}%";
            query = query.Where(x => EF.Functions.Like(x.recipient_phone, pattern));
        }

        if (request.OrderStatus.HasValue && request.OrderStatus > 0)
        {
            query = query.Where(x => x.order_status == request.OrderStatus.Value);
        }

        if (request.DeliveryTime.HasValue)
        {
            var searchDate = request.DeliveryTime.Value.Date;
            query = query.Where(x => x.delivery_time.HasValue && x.delivery_time.Value.Date == searchDate);
        }

        if (request.IsPaid.HasValue)
        {
            query = query.Where(x => x.is_paid == request.IsPaid.Value);
        }

        var count = await query.CountAsync();
        var totalPage = count > 0 ? (int)Math.Ceiling(count / (double)request.PageSize) : 0;
        var skip = (request.PageNo - 1) * request.PageSize;

        var data = await query
            .OrderByDescending(x => x.date_created)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(x => new
            {
                Id = x.id,
                CreatedAt = x.date_created ?? DateTime.MinValue,
                RecipientName = x.recipient_name,
                RecipientPhone = x.recipient_phone,
                TotalAmount = x.total_amount,
                StatusCode = x.order_status ?? 0,
                OrderStatus = x.order_status,
                IsPaid = x.is_paid,
                IsDelete = x.is_delete,
                DeliveryType = x.delivery_type
            })
            .ToListAsync();

        var result = new DataListResponse
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalPage = totalPage,
            Data = data
        };

        return new APIResponse(result);
    }

    public async Task<APIResponse> PublishFromAdminOrderAsync(OrderRequest request, string username)
    {
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING_OR_INVALID");
        }
        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null)
        {
            return new APIResponse("ERROR_ORDER_NOT_FOUND");
        }
        try
        {
            order.order_status = 1; // published
            order.date_updated = DateTime.Now;
            order.user_updated = username;
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("SUCCESS_ORDER_PUBLISHED");
        }

        return new APIResponse(200);
    }

    public async Task<APIResponse> UpdateStatusOrderAsync(OrderRequest request, string username)
    {
        if (request.Id == null || request.Id == Guid.Empty)
        {
            return new APIResponse("ERROR_ID_MISSING_OR_INVALID");
        }
        if (request.OrderStatus == null)
        {
            return new APIResponse("ERROR_ORDER_STATUS_MISSING_OR_INVALID");
        }
        var order = await _context.Orders.FindAsync(request.Id);
        if (order == null)
        {
            return new APIResponse("ERROR_ORDER_NOT_FOUND");
        }
        try
        {
            order.order_status = request.OrderStatus ?? order.order_status;
            order.date_updated = DateTime.Now;
            order.user_updated = username;
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new APIResponse("SUCCESS_ORDER_STATUS_UPDATED");
        }

        return new APIResponse(200);
    }
}
