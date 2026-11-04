using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MVC.Areas.Auth.Models.Services;
using MVC.Models;
using MVC.Models.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var configuredConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(configuredConnection))
{
    if (!builder.Environment.IsDevelopment())
    {
        throw new InvalidOperationException(
            "ConnectionStrings:DefaultConnection must be configured outside Development.");
    }

    var localDataDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
    Directory.CreateDirectory(localDataDirectory);
    configuredConnection = $"Data Source={Path.Combine(localDataDirectory, "site.db")}";
}

builder.Services.AddDbContext<SiteContext>(options =>
    options.UseSqlite(configuredConnection));

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    options.Password.RequiredLength = 15;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<SiteContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Auth/Account/Login");
    options.LogoutPath = new PathString("/Auth/Account/Logout");
    options.AccessDeniedPath = new PathString("/Auth/Account/AccessDenied");
});

builder.Services.AddScoped<UserInfoService>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<HomeService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<AdminService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
var englishOnlyLocalization = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures("en-US")
    .AddSupportedUICultures("en-US");
englishOnlyLocalization.RequestCultureProviders.Clear();
app.UseRequestLocalization(englishOnlyLocalization);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();