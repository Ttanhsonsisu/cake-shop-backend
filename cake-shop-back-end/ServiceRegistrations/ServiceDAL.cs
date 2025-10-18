using cake_shop_back_end.DataAccess.Cms.Auth;
using cake_shop_back_end.DataAccess.Cms.Common;
using cake_shop_back_end.DataAccess.Cms.MasterData;
using cake_shop_back_end.Extensions;
using cake_shop_back_end.Helpers;
using cake_shop_back_end.Interfaces.Cms.Auth;
using cake_shop_back_end.Interfaces.Common;
using cake_shop_back_end.Interfaces.MasterData;
using cake_shop_back_end.Models.Common;

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
        


    }
}
