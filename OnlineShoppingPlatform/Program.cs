using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.Carts;
using OnlineShoppingPlatform.Infrastructure.Data;
using OnlineShoppingPlatform.Infrastructure.Helpers;
using OnlineShoppingPlatform.Middleware;
using OnlineShoppingPlatform.Orders;
using OnlineShoppingPlatform.Products;
using OnlineShoppingPlatform.Users;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
}).AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });
builder.Services
    .AddProductsModule()
    .AddCartsModule()
    .AddOrdersModule()
    .AddUsersModule();

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DecryptedIdModelBinderProvider(
        builder.Services.BuildServiceProvider().GetRequiredService<IEncryptionHelper>()));
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ShoppingApp_";
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ShoppingWeb/dist/shopping-web";
});
builder.Services.AddDbContext<ShoppingDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Shopping")).EnableSensitiveDataLogging());

builder.Services.AddSingleton<IEncryptionHelper, EncryptionHelper>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseSpaStaticFiles();
app.MapControllers();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ShoppingWeb";
});

app.Run();
