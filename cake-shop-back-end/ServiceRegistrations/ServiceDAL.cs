using cake_shop_back_end.DataAccess.Cms.Auth;
using cake_shop_back_end.DataAccess.Cms.BusinessOperation;
using cake_shop_back_end.DataAccess.Cms.Common;
using cake_shop_back_end.DataAccess.Cms.Configuration;
using cake_shop_back_end.DataAccess.Cms.Customer;
using cake_shop_back_end.DataAccess.Cms.Marketing;
using cake_shop_back_end.DataAccess.Cms.MasterData;
using cake_shop_back_end.DataAccess.Cms.Order;
using cake_shop_back_end.DataAccess.Cms.Product;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using cake_shop_back_end.Interfaces.Cms.BusinessOperation;
using cake_shop_back_end.Interfaces.Cms.Configuration;
using cake_shop_back_end.Interfaces.Cms.Marketing;
using cake_shop_back_end.Interfaces.Cms.Order;
using cake_shop_back_end.Interfaces.Cms.Product;
using cake_shop_back_end.Interfaces.Common;
using cake_shop_back_end.Interfaces.MasterData;

namespace cake_shop_back_end.ServiceRegistrations;

public static class ServiceDAL
{
    public static void AddDalServices(this IServiceCollection services, string key, IConfiguration configuration)
    {
        // Register your data access layer services here
        // services.AddScoped<IYourRepository, YourRepositoryImplementation>();
        services.AddScoped<IEncryptData, EncryptData>();

        // authen 
        services.AddSingleton<IJwtAuth>(new Authen(key));
        services.AddScoped<IAction, ActionDataAccess>();
        services.AddScoped<IUser, UserDataAccess>();
        services.AddScoped<IFunction, FunctionDataAccess>();
        services.AddScoped<IUserGroup, UserGroupDataAccess>();
        services.AddScoped<ICustomer, CustomerDataAccess>();



        // common 
        services.AddScoped<ICommonFunction, CommonFunction>();
        services.AddScoped<ILoggingHelpers, LoggingHelpers>();
        services.AddScoped<ILogging, LoggingDataAccess>();
        
        // cms master data
        services.AddScoped<IProvince, ProvinceDataAccess>();
        services.AddScoped<IOtherList, OtherListDataAccess>();
        services.AddScoped<IOtherListType, OtherListTypeDataAccess>();
        services.AddScoped<IVersionApp, VersionAppDataAccess>();

        // extension 
        services.AddScoped<IEmailSender, EmailSender>();

        // cms config
        services.AddScoped<IDeliverySlot, DeliverySlotDataAccess>();
        services.AddScoped<IDeliveryArea, DeliveryAreaDataAccess>();
        services.AddScoped<IPaymentMethod, PaymentMethodDataAccess>();

        //cms core manager
        services.AddScoped<ICategory, CategoryDataAccess>();
        services.AddScoped<IAttribute,  AttributeDataAccess>();
        services.AddScoped<IAttributeValue, AttributeValueDataAccess>();
        services.AddScoped<IProductCake, ProductCakeDataAccess>();
        services.AddScoped<IVariant, VariantDataAccess>();
        services.AddScoped<ICategoryCustomOrder, CategoryCustomOrderDataAccess>();
        services.AddScoped<ICustomOrderOption, CustomOrderOptionDataAccess>();
        services.AddScoped<IOrder, OrderDataAccess>();
      
        // cms business operation
        services.AddScoped<IWebsiteImage, WebsiteImageDataAccess>();



        // cms marketting
        services.AddScoped<ICampaign, CampaignDataAccess>();
        services.AddScoped<IDiscountCode, DiscountCodeDataAccess>();

    }
}
