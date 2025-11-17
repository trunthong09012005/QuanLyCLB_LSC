using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;

namespace QuanLyCLB_LSC.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly QlClbLscContext _context;

        public UserDashboardController(QlClbLscContext context)
        {
            _context = context;
        }

        public IActionResult User(int maTV)
        {
            var thanhVien = _context.ThanhViens
                             .FirstOrDefault(tv => tv.MaTv == maTV);

            if (thanhVien == null)
            {
                return NotFound();
            }

            var model = new UserDashboardViewModel
            {
                ThanhVien = thanhVien,
                HoatDongs = _context.HoatDongs
                             .Where(h => h.NguoiPhuTrach == maTV)
                             .ToList(),

                // Sửa chỗ này: dùng property đúng tên của navigation trong PhanCong
                DuAns = _context.PhanCongs
                         .Where(p => p.MaTv == maTV)
                         .Select(p => p.MaDaNavigation) // <-- đổi từ p.DuAn thành p.MaDaNavigation
                         .ToList(),

                ThuChis = _context.ThuChis
                          .Where(t => t.NguoiThucHien == maTV)
                          .ToList(),

                LichHops = _context.LichHops
                           .Where(l => l.NguoiChuTri == maTV)
                           .ToList(),

                ThongBaos = _context.ThongBaos.ToList(),

                TinNhans = _context.TinNhans
                          .Where(tn => tn.MaNguoiNhan == maTV)
                          .ToList()
            };

            return View(model);
        }
    }
}
