using MVC.Models;
using MVC.Models.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ������ �������� MVC
builder.Services.AddControllersWithViews();

// �������� �������� ���� ����� � SQLite
builder.Services.AddDbContext<SiteContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=site.db"));

// ���������� ������, �� ����� ������������� ����� EF
builder.Services.AddScoped<UserInfoService>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<HomeService>();

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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
