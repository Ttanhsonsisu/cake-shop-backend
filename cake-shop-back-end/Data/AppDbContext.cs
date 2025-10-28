using cake_shop_back_end.Models.auth;
using cake_shop_back_end.Models.CakeProduct;
using cake_shop_back_end.Models.Common;
using cake_shop_back_end.Models.Discount;
using cake_shop_back_end.Models.FeedBack;
using cake_shop_back_end.Models.Inventory;
using cake_shop_back_end.Models.Invoice;
using cake_shop_back_end.Models.MasterData;
using cake_shop_back_end.Models.Order;
using cake_shop_back_end.Models.Partner;
using cake_shop_back_end.Models.Payment;
using cake_shop_back_end.Models.Post;
using cake_shop_back_end.Models.Refund;
using cake_shop_back_end.Models.Seasonal;
using cake_shop_back_end.Models.Shipment;
using Microsoft.EntityFrameworkCore;
using Attribute = cake_shop_back_end.Models.CakeProduct.Attribute;

namespace cake_shop_back_end.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // ===================================
    // 1. user management & permission : done
    // ===================================

    public DbSet<User> Users { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<UserGroupPermission> UserGroupPermissions { get; set; }
    public DbSet<Function> Functions { get; set; }
    public DbSet<Action1> Actions { get; set; }
    public DbSet<AddressInfo> AddressInfos { get; set; }
    public DbSet<Customer> Customers { get; set; }

    // ===================================
    // 2. product management : done
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
    // 3. inventory & warehouse
    // ===================================

    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<InventoryIngredient> InventoryIngredients { get; set; }

    // ===================================
    // 4. order & shipment
    // ===================================

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<DeliverySchedule> DeliverySchedules { get; set; }
    public DbSet<DeliverySetting> DeliverySettings { get; set; }
    public DbSet<CatogoryCustomOrder> CatogoryCustomOrders { get; set; }
    public DbSet<CustomOrderDetail> CustomOrderDetails { get; set; }
    public DbSet<CustomOrderOption> CustomOrderOptions { get; set; }

    // ===================================
    // 5. post & Feedback
    // ===================================

    public DbSet<Post> Posts { get; set; }
    public DbSet<PostCategory> PostCategories { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<PostTagMapping> PostTagMappings { get; set; }
    public DbSet<PostComment> PostComments { get; set; }

    // ===================================
    // 6. Common info & System
    // ===================================

    public DbSet<DeliveryArea> DeliveryAreas { get; set; }
    public DbSet<DeliveryTimeSlot> DeliveryTimeSlots { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationContent> NotificationContents { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<WebSiteImgage> WebSiteImgages { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<Logging> Loggings { get; set; }
    public DbSet<OtherList> OtherLists { get; set; }
    public DbSet<OtherListType> OtherListTypes { get; set; }
    public DbSet<VersionApp> VersionApps { get; set; }

    // ===================================
    // 7. Discount
    // ===================================
    
    public DbSet<DiscountCampaign> DiscountCampaigns { get; set; }
    public DbSet<DiscountTarget> DiscountTargets { get; set; }
    public DbSet<DiscountCode> DiscountCodes { get; set; }
    public DbSet<DiscountCodeTarget> DiscountCodeTargets { get; set; }

    // ===================================
    // 8. Customer & Review
    // ===================================

    public DbSet<Review> Reviews { get; set; }

    // ==================================
    // 8. Invoice & Billing 
    // ==================================

    public DbSet<Invoice> Invoices { get; set; }

    // ==================================
    // 10. Partner & Supplier
    // ==================================

    public DbSet<PartnerInvoice> PartnerInvoices { get; set; }
    public DbSet<PartnerDelivery> PartnerDeliveries { get; set; }
    public DbSet<DeliveryZone> DeliveryZones { get; set; }

    // ===================================
    // 11. Payment 
    // ===================================

    public DbSet<Payment> Paymentes { get; set; }

    // ===================================
    // 12. Refund
    // ===================================

    public DbSet<RefundRequest> RefundRequests { get; set; }

    // ===================================
    // 13.Seasonal Event
    // ===================================

    public DbSet<SeasonalEvent> SeasonalEvents { get; set; }
    public DbSet<SeasonalEventIncludeCategory> SeasonalEventIncludeCategories { get; set; }
    public DbSet<SeasonalEventIncludeProduct> SeasonalEventIncludeProducts { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1.1. User Management & Permission
        modelBuilder.Entity<User>().ToTable("User").HasKey(v => v.id);
        modelBuilder.Entity<UserGroup>().ToTable("UserGroup").HasKey(v => v.id);
        modelBuilder.Entity<UserPermission>().ToTable("UserPermission").HasKey(v => v.id);
        modelBuilder.Entity<UserGroupPermission>().ToTable("UserGroupPermission").HasKey(v => v.id);
        modelBuilder.Entity<Function>().ToTable("Function").HasKey(v => v.id);
        modelBuilder.Entity<Action1>().ToTable("Action").HasKey(v => v.id); 
        modelBuilder.Entity<AddressInfo>().ToTable("AddressInfo").HasKey(v => v.id);
        modelBuilder.Entity<Customer>().ToTable("Customer").HasKey(v => v.id);

        // 1.2. Product Management
        modelBuilder.Entity<Category>().ToTable("Category").HasKey(v => v.id);
        modelBuilder.Entity<Tag>().ToTable("Tag").HasKey(v => v.id);
        modelBuilder.Entity<ProductCake>().ToTable("ProductCake").HasKey(v => v.id);
        modelBuilder.Entity<ProductCakeCategory>().ToTable("ProductCakeCategory").HasKey(v => v.id);
        modelBuilder.Entity<ProductImage>().ToTable("ProductImages").HasKey(v => v.image_id);
        modelBuilder.Entity<Variant>().ToTable("Variant").HasKey(v => v.id);
        modelBuilder.Entity<VariantImage>().ToTable("VariantImage").HasKey(v => v.variant_image_id);
        modelBuilder.Entity<Attribute>().ToTable("Attribute").HasKey(v => v.id);
        modelBuilder.Entity<AttributeValue>().ToTable("AttributeValue").HasKey(v => v.id);
        modelBuilder.Entity<VariantAttributeValue>().ToTable("VariantAttributeValue").HasKey(v => v.id);

        // 1.3. Inventory & Warehouse
        modelBuilder.Entity<Inventory>().ToTable("Inventory").HasKey(v => v.id);
        modelBuilder.Entity<InventoryTransaction>().ToTable("InventoryTransaction").HasKey(v => v.id);
        modelBuilder.Entity<InventoryIngredient>().ToTable("InventoryIngredient").HasKey(v => v.id);  

        // 1.4. Order & Shipment
        modelBuilder.Entity<Order>().ToTable("Order").HasKey(v => v.id); 
        modelBuilder.Entity<OrderItem>().ToTable("OrderItem").HasKey(v => v.id);
        modelBuilder.Entity<Payment>().ToTable("Payment").HasKey(v => v.id);
        modelBuilder.Entity<Shipment>().ToTable("Shipment").HasKey(v => v.id);
        modelBuilder.Entity<OrderStatusHistory>().ToTable("OrderStatusHistory").HasKey(v => v.id);
        modelBuilder.Entity<DeliverySchedule>().ToTable("DeliverySchedule").HasKey(v => v.id); 
        modelBuilder.Entity<DeliverySetting>().ToTable("DeliverySetting").HasKey(v => v.id); 
        modelBuilder.Entity<CatogoryCustomOrder>().ToTable("CatogoryCustomOrder").HasKey(v => v.id);  
        modelBuilder.Entity<CustomOrderDetail>().ToTable("CustomOrderDetail").HasKey(v => v.id);  
        modelBuilder.Entity<CustomOrderOption>().ToTable("CustomOrderOption").HasKey(v => v.id);  

        // 1.5. Post & Feedback
        modelBuilder.Entity<Post>().ToTable("Post").HasKey(v => v.id);
        modelBuilder.Entity<PostCategory>().ToTable("PostCategory").HasKey(v => v.id);
        modelBuilder.Entity<PostTag>().ToTable("PostTag").HasKey(v => v.id);
        modelBuilder.Entity<PostComment>().ToTable("PostComment").HasKey(v => v.id);
        // Cấu hình khóa chính composite cho bảng trung gian
        modelBuilder.Entity<PostTagMapping>()
            .ToTable("PostTagMapping")
            .HasKey(ptm => new { ptm.post_id, ptm.tag_id });

        // 1.6. Common info & System
        modelBuilder.Entity<DeliveryArea>().ToTable("DeliveryArea").HasKey(v => v.id);  
        modelBuilder.Entity<DeliveryTimeSlot>().ToTable("DeliveryTimeSlot").HasKey(v => v.id);  
        modelBuilder.Entity<Notification>().ToTable("Notification").HasKey(v => v.id);  
        modelBuilder.Entity<NotificationContent>().ToTable("NotificationContent").HasKey(v => v.id);  
        modelBuilder.Entity<PaymentMethod>().ToTable("PaymentMethod").HasKey(v => v.id);  
        modelBuilder.Entity<WebSiteImgage>().ToTable("WebSiteImgage").HasKey(v => v.id);  
        modelBuilder.Entity<Province>().ToTable("Province").HasKey(v => v.id);
        modelBuilder.Entity<Logging>().ToTable("Logging").HasKey(v => v.id);
        modelBuilder.Entity<OtherList>().ToTable("OtherList").HasKey(v => v.id);
        modelBuilder.Entity<OtherListType>().ToTable("OtherListType").HasKey(v => v.id);
        modelBuilder.Entity<VersionApp>().ToTable("VersionApp").HasKey(v => v.id);

        // 1.7. Discount
        modelBuilder.Entity<DiscountCampaign>().ToTable("DiscountCampaign").HasKey(v => v.id);
        modelBuilder.Entity<DiscountTarget>().ToTable("DiscountTarget").HasKey(v => v.id);
        modelBuilder.Entity<DiscountCode>().ToTable("DiscountCode").HasKey(v => v.id);  
        modelBuilder.Entity<DiscountCodeTarget>().ToTable("DiscountCodeTarget").HasKey(v => v.id);  

        // 1.8. Customer & Review
        modelBuilder.Entity<Review>().ToTable("Review").HasKey(v => v.id);

        // 1.9. Invoice & Billing
        modelBuilder.Entity<Invoice>().ToTable("Invoice").HasKey(v => v.id);  

        // 1.10. Partner & Supplier
        modelBuilder.Entity<PartnerInvoice>().ToTable("PartnerInvoice").HasKey(v => v.id);  
        modelBuilder.Entity<PartnerDelivery>().ToTable("PartnerDelivery").HasKey(v => v.id);  
        modelBuilder.Entity<DeliveryZone>().ToTable("DeliveryZone").HasKey(v => v.id);  

        // 1.11. Refund
        modelBuilder.Entity<RefundRequest>().ToTable("RefundRequest").HasKey(v => v.id);  

        // 1.12. Seasonal Event
        modelBuilder.Entity<SeasonalEvent>().ToTable("SeasonalEvent").HasKey(v => v.id);  
        modelBuilder.Entity<SeasonalEventIncludeCategory>().ToTable("SeasonalEventIncludeCategory").HasKey(v => v.id);  
        modelBuilder.Entity<SeasonalEventIncludeProduct>().ToTable("SeasonalEventIncludeProduct").HasKey(v => v.id); 

    }
}
