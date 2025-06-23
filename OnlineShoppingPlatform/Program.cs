using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform;
using OnlineShoppingPlatform.Data;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
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


app.UseAuthorization();

app.UseStaticFiles();
app.UseSpaStaticFiles();

app.MapControllers();

// Configure the SPA fallback for client-side routing
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ShoppingWeb";
    if (app.Environment.IsDevelopment())
    {
        // If dev, proxy to Angular CLI if desired
        spa.UseAngularCliServer(npmScript: "start");
    }
});

app.Run();
