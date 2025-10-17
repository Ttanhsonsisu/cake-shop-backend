using cake_shop_back_end.Models.auth;
using cake_shop_back_end.Models.CakeProduct;
using cake_shop_back_end.Models.Common;
using cake_shop_back_end.Models.FeedBack;
using cake_shop_back_end.Models.Inventory;
using cake_shop_back_end.Models.MasterData;
using cake_shop_back_end.Models.Order;
using cake_shop_back_end.Models.Payment;
using cake_shop_back_end.Models.Post;
using cake_shop_back_end.Models.Shipment;
using Microsoft.EntityFrameworkCore;
using Attribute = cake_shop_back_end.Models.CakeProduct.Attribute;

namespace cake_shop_back_end.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // ===================================
    // 1. Quản lý Người dùng & Phân quyền
    // ===================================

    public DbSet<User> Users { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<UserGroupPermission> UserGroupPermissions { get; set; }
    public DbSet<Function> Functions { get; set; }

    // Lưu ý: Đổi tên class 'Action' trong C# để tránh xung đột với System.Action
    // Nếu bạn đặt tên class C# là Action, bạn phải đổi tên DbSet, ví dụ:
    public DbSet<Action1> Actions { get; set; }
    // HOẶC: public DbSet<ActionModel> Actions { get; set; } // Nếu bạn đã đổi tên class C#

    // ===================================
    // 2. Quản lý Sản phẩm
    // ===================================

    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ProductCake> ProductCakes { get; set; }
    public DbSet<ProductCakeCategory> ProductCakeCategories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    public DbSet<Variant> Variants { get; set; }
    public DbSet<VariantImage> VariantImages { get; set; }
    public DbSet<Attribute> Attributes { get; set; }
    public DbSet<AttributeValue> AttributeValues { get; set; }
    public DbSet<VariantAttributeValue> VariantAttributeValues { get; set; }

    // ===================================
    // 3. Quản lý Kho hàng
    // ===================================

    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    // ===================================
    // 4. Quản lý Đơn hàng & Giao vận
    // ===================================

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<DeliverySchedule> DeliverySchedules { get; set; }
    public DbSet<DeliverySetting> DeliverySettings { get; set; }

    // ===================================
    // 5. Quản lý Bài viết & Nội dung
    // ===================================

    public DbSet<Post> Posts { get; set; }
    public DbSet<PostCategory> PostCategories { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<PostTagMapping> PostTagMappings { get; set; }
    public DbSet<PostComment> PostComments { get; set; }

    // ===================================
    // 6. Thông tin Chung & Hệ thống
    // ===================================

    public DbSet<Province> Provinces { get; set; }
    public DbSet<Logging> Loggings { get; set; }
    public DbSet<OtherList> OtherLists { get; set; }
    public DbSet<OtherListType> OtherListTypes { get; set; }
    public DbSet<VersionApp> VersionApps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure your entity mappings here
        // ===================================

        // 1. Cấu hình Khóa chính và Tên bảng
        // ===================================
        modelBuilder.Entity<PostTagMapping>()
        .ToTable("PostTagMapping")
        .HasKey(ptm => new { ptm.post_id, ptm.tag_id });

        // 1.1. Quản lý Người dùng & Phân quyền
        modelBuilder.Entity<User>().ToTable("User").HasKey(v => v.id);
        modelBuilder.Entity<UserGroup>().ToTable("UserGroup").HasKey(v => v.id);
        modelBuilder.Entity<UserPermission>().ToTable("UserPermission").HasKey(v => v.id);
        modelBuilder.Entity<UserGroupPermission>().ToTable("UserGroupPermission").HasKey(v => v.id);
        modelBuilder.Entity<Function>().ToTable("Function").HasKey(v => v.id);
        modelBuilder.Entity<Action1>().ToTable("Action").HasKey(v => v.id); // Lưu ý: Class C# Action, Table SQL [Action]

        // 1.2. Quản lý Sản phẩm
        modelBuilder.Entity<Category>().ToTable("Category").HasKey(v => v.id);
        modelBuilder.Entity<Tag>().ToTable("Tag").HasKey(v => v.id);
        modelBuilder.Entity<ProductCake>().ToTable("ProductCake").HasKey(v => v.id);
        modelBuilder.Entity<ProductCakeCategory>().ToTable("ProductCakeCategory").HasKey(v => v.id);
        modelBuilder.Entity<ProductImage>().ToTable("ProductImages").HasKey(v => v.image_id); // Khóa chính là image_id
        modelBuilder.Entity<VariantImage>().ToTable("VariantImage").HasKey(v => v.variant_image_id); // Khóa chính là variant_image_id
        modelBuilder.Entity<Variant>().ToTable("Variant").HasKey(v => v.id);
        modelBuilder.Entity<Attribute>().ToTable("Attribute").HasKey(v => v.id);
        modelBuilder.Entity<AttributeValue>().ToTable("AttributeValue").HasKey(v => v.id);
        modelBuilder.Entity<VariantAttributeValue>().ToTable("VariantAttributeValue").HasKey(v => v.id);

        // 1.3. Quản lý Kho hàng
        modelBuilder.Entity<Inventory>().ToTable("Inventory").HasKey(v => v.id);
        modelBuilder.Entity<InventoryTransaction>().ToTable("InventoryTransaction").HasKey(v => v.id);

        // 1.4. Quản lý Đơn hàng & Giao vận
        modelBuilder.Entity<Order>().ToTable("Order").HasKey(v => v.id); // Lưu ý: Class C# Order, Table SQL [Order]
        modelBuilder.Entity<OrderItem>().ToTable("OrderItem").HasKey(v => v.id);
        modelBuilder.Entity<Payment>().ToTable("Payment").HasKey(v => v.id);
        modelBuilder.Entity<Shipment>().ToTable("Shipment").HasKey(v => v.id);
        modelBuilder.Entity<OrderStatusHistory>().ToTable("OrderStatusHistory").HasKey(v => v.id);
        modelBuilder.Entity<DeliverySchedule>().ToTable("DeliverySchedules").HasKey(v => v.id);
        modelBuilder.Entity<DeliverySetting>().ToTable("DeliverySettings").HasKey(v => v.id);

        // 1.5. Quản lý Bài viết & Nội dung
        modelBuilder.Entity<Review>().ToTable("Review").HasKey(v => v.id);
        modelBuilder.Entity<PostCategory>().ToTable("PostCategory").HasKey(v => v.id);
        modelBuilder.Entity<Post>().ToTable("Post").HasKey(v => v.id);
        modelBuilder.Entity<PostTag>().ToTable("PostTag").HasKey(v => v.id);
        modelBuilder.Entity<PostComment>().ToTable("PostComment").HasKey(v => v.id);

        // 1.6. Thông tin Chung & Hệ thống
        modelBuilder.Entity<Province>().ToTable("Province").HasKey(v => v.id);
        modelBuilder.Entity<Logging>().ToTable("Logging").HasKey(v => v.id);
        modelBuilder.Entity<OtherList>().ToTable("OtherList").HasKey(v => v.id);
        modelBuilder.Entity<OtherListType>().ToTable("OtherListType").HasKey(v => v.id);
        modelBuilder.Entity<VersionApp>().ToTable("VersionApp").HasKey(v => v.id);
    }
}
