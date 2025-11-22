using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_CLB_LSC1.ViewModels;
using QuanLyCLB_LSC.Models;

namespace QL_CLB_LSC1.Controllers
{
    public class HomeController : Controller
    {
        private readonly QlClbLscContext _context;

        public HomeController(QlClbLscContext context)
        {
            _context = context;
        }

        // Action mặc định khi vào trang chủ
        [Route("")]
        [Route("Home")]
        public async Task<IActionResult> Home()
        {
            var viewModel = new HomeViewModel
            {
                // Lấy thống kê từ database
                // Điều chỉnh tên DbSet theo đúng tên trong QlClbLscContext của bạn
                TotalMembers = await _context.ThanhViens.CountAsync(),
                TotalActivities = await _context.HoatDongs.CountAsync(),
                TotalDepartments = await _context.BanChuyenMons.CountAsync(),

                // Lấy hình ảnh từ các hoạt động gần đây
                GalleryItems = await _context.HoatDongs
                    .Include(h => h.MaLoaiHdNavigation)
                    .OrderByDescending(h => h.NgayToChuc)
                    .Take(6)
                    .Select(h => new GalleryItem
                    {
                        Id = h.MaHd,
                        ImageUrl = GetActivityImage(h.MaLoaiHd),
                        Title = h.TenHd,
                        Badge = h.MaLoaiHdNavigation != null
                            ? h.MaLoaiHdNavigation.TenLoaiHd
                            : "Hoạt động",
                        Location = h.DiaDiem ?? "CLB",
                        Date = h.NgayToChuc.HasValue
                            ? h.NgayToChuc.Value.ToString("MM/yyyy")
                            : ""
                    })
                    .ToListAsync(),

                // Hoạt động nổi bật theo loại
                ActivityPreviews = new List<ActivityPreview>
                {
                    new ActivityPreview
                    {
                        Id = 1,
                        Title = "Hoạt động Tình nguyện",
                        Description = "Tham gia các chiến dịch vì cộng đồng, mang lại giá trị tích cực cho xã hội",
                        Icon = "fa-heart",
                        GradientClass = "gradient-pink"
                    },
                    new ActivityPreview
                    {
                        Id = 2,
                        Title = "Workshop Học thuật",
                        Description = "Nâng cao kiến thức và kỹ năng qua các buổi chia sẻ từ chuyên gia",
                        Icon = "fa-lightbulb",
                        GradientClass = "gradient-blue"
                    },
                    new ActivityPreview
                    {
                        Id = 3,
                        Title = "Team Building",
                        Description = "Gắn kết thành viên thông qua các hoạt động vui chơi và dã ngoại",
                        Icon = "fa-users",
                        GradientClass = "gradient-green"
                    }
                }
            };

            return View(viewModel);
        }

        // Helper method để lấy ảnh theo loại hoạt động
        private static string GetActivityImage(int? maLoaiHD)
        {
            return maLoaiHD switch
            {
                1 => "https://images.unsplash.com/photo-1559027615-cd4628902d4a?w=600", // Tình nguyện
                2 => "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=600", // Học thuật
                3 => "https://images.unsplash.com/photo-1529156069898-49953e39b3ac?w=600", // Giải trí
                _ => "https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=600"  // Mặc định
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}