using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC;
using QuanLyCLB_LSC.Models;

var builder = WebApplication.CreateBuilder(args);

// ============================
// 1️⃣ Cấu hình DbContext (SQL Server)
// ============================
builder.Services.AddDbContext<QlClbLscContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn"))
);

// ============================
// 2️⃣ Thêm MVC (Controller + View)
// ============================
builder.Services.AddControllersWithViews();

// ============================
// 3️⃣ Thêm Session
// ============================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1); // thời gian tồn tại session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ============================
// 4️⃣ Pipeline xử lý request
// ============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⚠️ Bật session trước Authorization
app.UseSession();

app.UseAuthorization();

// ============================
// 5️⃣ Cấu hình route mặc định
// ============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

// ============================
// 6️⃣ Chạy ứng dụng
// ============================
app.Run();
