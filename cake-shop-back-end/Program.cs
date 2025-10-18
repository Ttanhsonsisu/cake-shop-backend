using System.Security.Claims;
using cake_shop_back_end.Data;
using cake_shop_back_end.ServiceRegistrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// enable CORS  
builder.Services.AddCors(builder =>
{
    builder.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//JSON Serializer
builder.Services.AddControllers().AddNewtonsoftJson(option =>

    option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
    }
);

// add connection database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// add redis cache
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = "localhost";
//    options.InstanceName = "CakeShopInstance";
//});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// add auth
string key = builder.Configuration.GetValue<string>("SecretToken");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
    };
});

// add authorization

builder.Services.AddAuthorizationBuilder()
                       // add authorization
                       .AddPolicy("WebAdminUser", policy => policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier 
                        && c.Value == "web_admin")
    ))
                        // add authorization
                        .AddPolicy("WebMerchantUser", policy => policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier 
                        && (c.Value == "web_partner"))
    ))
                        // add authorization
                        .AddPolicy("AppPartner", policy => policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier 
                        && c.Value == "app_partner")
    ))
                        // add authorization
                        .AddPolicy("WebAdminMerchantUser", policy => policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier 
                        && (c.Value == "web_admin" || c.Value == "web_partner"))
    ));

builder.Services.AddHttpClient("HttpClientWithSSLUntrusted").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ClientCertificateOptions = ClientCertificateOption.Manual,
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});

// add DAL services
builder.Services.AddDalServices(key, builder.Configuration);

// add redis cache
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    try
//    {
//        var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
//        return ConnectionMultiplexer.Connect(redisConnection);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Redis connection failed: {ex.Message}");
//        // Return a mock or null object to avoid startup failure
//        return null; // hoặc: return Mock.Of<IConnectionMultiplexer>();
//    }
//});

// add BLL
builder.Services.AddBLLServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();

//CreateDataBaseIfNotExists(builder.Configuration);
DbInitializer.Initialize(app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>());
app.Run();