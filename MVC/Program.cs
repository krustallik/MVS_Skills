using MVC.Models;
using MVC.Models.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Додаємо підтримку MVC
builder.Services.AddControllersWithViews();

// Реєструємо контекст бази даних з SQLite
builder.Services.AddDbContext<SiteContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=site.db"));

// Зареєструємо сервіси, які тепер працюватимуть через EF
builder.Services.AddScoped<UserInfoService>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<HomeService>();

var app = builder.Build();

// Конфігурація конвеєра обробки запитів
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
