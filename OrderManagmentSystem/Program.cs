using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.Extensions.Configuration;
using OrderManagmentSytemBAL.CustomerRepositry;
using OrderManagmentSytemBAL.OrderRepositry;
using OrderManagmentSytemBAL.Services.Auth;
using OrderManagmentSytemBAL.UserRepositry;
using OrderManagmentSytemDAL.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICustomerRepositry,CustomerRepositry>();
builder.Services.AddScoped<IOrderRepositry, OrderRepositry>();
builder.Services.AddScoped<IUserRepositry, UserRespositry>();
builder.Services.AddScoped<IJWTService,JWTService>();
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
}
);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");



    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseNotyf();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
