using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform;
using OnlineShoppingPlatform.Data;

var builder = WebApplication.CreateBuilder(args);
// Add authentication services with necessary schemes
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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ShoppingWeb/dist/shopping-web";
});
builder.Services.AddDbContext<ShoppingDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Shopping")));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseSpaStaticFiles();
app.MapControllers();
//app.MapFallbackToFile("index.html");

// Configure the SPA fallback for client-side routing
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
