namespace cake_shop_back_end.ServiceRegistrations;

public static class ServiceDAL
{
    public static void AddDalServices(this IServiceCollection services, string key, IConfiguration configuration)
    {
        // Register your data access layer services here
        // services.AddScoped<IYourRepository, YourRepositoryImplementation>();
    }
}
