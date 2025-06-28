using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform;
using OnlineShoppingPlatform.Data;
using OnlineShoppingPlatform.Helpers;
using OnlineShoppingPlatform.Middleware;
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
builder.Services.AddCustomDependencies();
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DecryptedIdModelBinderProvider(
        builder.Services.BuildServiceProvider().GetRequiredService<IEncryptionHelper>()));
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
    //if (app.Environment.IsDevelopment())
    //{
    //    // If dev, proxy to Angular CLI if desired
    //    spa.UseProxyToSpaDevelopmentServer("http://localhost:8000");
    //}
});

app.Run();
