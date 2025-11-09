using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC.Models;

namespace QuanLyCLB_LSC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🔗 1. Cấu hình DbContext (kết nối SQL Server)
            builder.Services.AddDbContext<QlClbLscContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn"))
            );

            // 🌐 2. Thêm MVC (Controller + View)
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // ⚙️ 3. Cấu hình pipeline xử lý request
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Bật HTTPS Strict Transport Security (bảo mật)
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Để đọc file CSS, JS trong wwwroot
            app.UseRouting();
            app.UseAuthorization();

            // 🧭 4. Cấu hình route mặc định
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}"
            );

            // 🚀 5. Chạy ứng dụng
            app.Run();
        }
    }
}
