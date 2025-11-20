using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Order;
using cake_shop_back_end.DataObjects.Responses;
using cake_shop_back_end.Interfaces.Cms.Order;
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
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var newOrderId = Guid.NewGuid();

            var orderItemsEntities = new List<OrderItem>();
            decimal calculatedTotalAmount = 0;

            if (request.OrderItems != null && request.OrderItems.Any())
            {
                foreach (var itemReq in request.OrderItems)
                {
                    // Validate dữ liệu cơ bản
                    var quantity = itemReq.Quantity ?? 1;
                    var unitPrice = itemReq.UnitPrice ?? 0;
                    var itemTotal = quantity * unitPrice;

                    calculatedTotalAmount += itemTotal;

                    var orderItem = new OrderItem
                    {
                        id = Guid.NewGuid(),
                        order_id = newOrderId,
                        product_id = itemReq.ProductId,
                        variant_id = itemReq.VariantId,
                        quantity = quantity,
                        price = unitPrice,
                        total_price = itemTotal,

                        date_created = DateTime.Now,
                        user_created = username,
                        date_updated = DateTime.Now,
                        user_updated = username
                    };
                    orderItemsEntities.Add(orderItem);
                }
            }

            var orderEntity = new cake_shop_back_end.Models.Order.Order
            {
                id = newOrderId,
                user_id = request.UserId ?? Guid.Empty,
                recipient_name = request.RecipientName,
                recipient_phone = request.RecipientPhone,
                recipient_address = request.RecipientAddress,
                delivery_type = request.DeliveryType,
                name_guest = request.NameGuest,
                phone_guest = request.PhoneGuest,
                email_guest = request.EmailGuest,
                order_tag = request.OrderTag,
                delivery_time = request.DeliveryTime,
                order_source = request.OrderSource ?? 0, 

                // Logic tính tiền: Tổng hàng + Phí ship
                delivery_fee = request.DeliveryFee ?? 0,
                total_amount = calculatedTotalAmount + (request.DeliveryFee ?? 0),

                payment_method = request.PaymentMethod ?? 0, 
                order_status = request.OrderStatus ?? -1,    

                is_paid = request.IsPaid ?? false,
                is_custom_order = request.IsCustomOrder ?? false,
                is_delete = false,

                // Các trường audit từ MasterCommonModel
                date_created = DateTime.Now,
                user_created = username,
                date_updated = DateTime.Now,
                user_updated = username
            };

            await _context.Orders.AddAsync(orderEntity);

            if (orderItemsEntities.Any())
            {
                await _context.OrderItems.AddRangeAsync(orderItemsEntities);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new APIResponse(200);
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
