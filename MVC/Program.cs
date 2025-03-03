using MVC.Models;
using MVC.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MVC.Areas.Auth.Models.Services;

var builder = WebApplication.CreateBuilder(args);

// ������ �������� MVC
builder.Services.AddControllersWithViews();

// �������� �������� ���� ����� � SQLite
builder.Services.AddDbContext<SiteContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=site.db"));

builder.Services.AddIdentity<User, IdentityRole<int>>(opt => { 
    opt.SignIn.RequireConfirmedAccount = false; //���� ��� ������������� �������� 
    opt.SignIn.RequireConfirmedEmail = false;//                             �����
    opt.SignIn.RequireConfirmedPhoneNumber = false;//                   �� ��������

    opt.Password.RequiredLength = 3;
    opt.Password.RequiredUniqueChars = 0;
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false; //�������� ����� �� ������
})  .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<SiteContext>();

builder.Services.ConfigureApplicationCookie(options => //������������ �������� ���� ���������� ������������
{
    options.LoginPath = new PathString("/Auth/Account/Login");
    options.LogoutPath = new PathString("/Auth/Account/Logout");
    options.AccessDeniedPath = new PathString("/Auth/Account/AccessDenied");
});


// ���������� ������, �� ����� ������������� ����� EF
builder.Services.AddScoped<UserInfoService>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<HomeService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<AdminService>();

var app = builder.Build();

// ������������ ������� ������� ������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
