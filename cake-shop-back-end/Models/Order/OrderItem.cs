using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.Order;

public class OrderItem : MasterCommonModel
{
    public Guid id { get; set; } = new Guid();
    public Guid order_id { get; set; }
    public Guid? product_id { get; set; }
    public Guid? variant_id { get; set; }
    public int quantity { get; set; } = 1;
    public decimal price { get; set; }

    // Lưu ý: total_price là cột tính toán (AS PERSISTED) trong SQL.
    // Trong C# Model, ta thường định nghĩa nó là thuộc tính chỉ đọc (readonly) 
    // hoặc tính toán lại giá trị dựa trên các thuộc tính khác.
    // Nếu sử dụng Entity Framework, bạn có thể cấu hình nó là cột tính toán 
    // trong DbContext (OnModelCreating) hoặc loại trừ khỏi mapping (Ignore).
    public decimal total_price { get; set; }
}