
using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Order;

public class OrderRequest : PaggingRequest
{
    public Guid? Id { get; set; } = new Guid();
    public Guid? UserId { get; set; }
    public string? RecipientName { get; set; }
    public string? OrderNo { get; set; } = string.Empty;
    public string? RecipientPhone { get; set; }
    public string? RecipientAddress { get; set; }
    public string? DeliveryType { get; set; }
    public string? NameGuest { get; set; }
    public string? PhoneGuest { get; set; }
    public string? EmailGuest { get; set; }
    public int? OrderTag { get; set; }
    public DateTime? DeliveryTime { get; set; }
    public int? OrderSource { get; set; }
    public decimal? DeliveryFee { get; set; } = 0m;
    public decimal? TotalAmount { get; set; }
    public int? PaymentMethod { get; set; }
    public int? OrderStatus { get; set; }
    public bool? IsPaid { get; set; } 
    public bool? IsCustomOrder { get; set; } = false;
    public decimal? SubtotalAmount { get; set; }
    public decimal? DiscountAmount { get; set; } = 0m;
    public decimal? TaxAmount { get; set; } = 0m;
    public string? DiscountCode { get; set; } = string.Empty;
    public List<OrderItemRequest>? OrderItems { get; set; } = new List<OrderItemRequest>();
}

public class OrderItemRequest
{
    public Guid? ProductId { get; set; }
    public Guid? VariantId { get; set; }
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? product_name { get; set; } 
    public string? variant_name { get; set; } 
}

