using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> User()
        {
            // Lấy MaTV từ session hoặc claims
            int maTV = HttpContext.Session.GetInt32("MaTV") ?? 0;


            var viewModel = new UserDashboardViewModel();

            // 1. Lấy thông tin thành viên
            var thanhVien = await _context.ThanhViens
                .Include(tv => tv.MaCvNavigation)
                .Include(tv => tv.MaBanNavigation)
                .FirstOrDefaultAsync(tv => tv.MaTv == maTV);

            if (thanhVien == null)
            {
                return NotFound("Không tìm thấy thông tin thành viên");
            }

            viewModel.MaTV = thanhVien.MaTv;
            viewModel.HoTen = thanhVien.HoTen;
            viewModel.NgaySinh = thanhVien.NgaySinh?.ToDateTime(TimeOnly.MinValue);
            viewModel.GioiTinh = thanhVien.GioiTinh;
            viewModel.Lop = thanhVien.Lop;
            viewModel.Khoa = thanhVien.Khoa;
            viewModel.SDT = thanhVien.Sdt;
            viewModel.Email = thanhVien.Email;
            viewModel.DiaChi = thanhVien.DiaChi;
            viewModel.VaiTro = thanhVien.VaiTro;
            viewModel.NgayThamGia = thanhVien.NgayThamGia?.ToDateTime(TimeOnly.MinValue);
            viewModel.TrangThai = thanhVien.TrangThai;
            viewModel.TenChucVu = thanhVien.MaCvNavigation?.TenCv;
            viewModel.TenBan = thanhVien.MaBanNavigation?.TenBan;

            // 2. Lấy điểm rèn luyện mới nhất
            var diemRL = await _context.DiemRenLuyens
                .Where(d => d.MaTv == maTV)
                .OrderByDescending(d => d.NamHoc)
                .ThenByDescending(d => d.HocKy)
                .FirstOrDefaultAsync();

            if (diemRL != null)
            {
                viewModel.DiemRenLuyen = diemRL.Diem;
                viewModel.HocKy = diemRL.HocKy;
                viewModel.NamHoc = diemRL.NamHoc;
            }

            // 3. Thống kê tổng quan
            viewModel.TongThanhVien = await _context.ThanhViens.CountAsync();
            viewModel.TongSuKien = await _context.HoatDongs.CountAsync();
            viewModel.TongDuAn = await _context.DuAns.CountAsync();
            viewModel.SoGiaiThuong = await _context.KhenThuongs
                .Where(kt => kt.MaTv == maTV)
                .CountAsync();
            // 4. Danh sách hoạt động (9 hoạt động gần nhất)
            var danhSachHoatDongDb = await _context.HoatDongs
                .Include(hd => hd.MaLoaiHdNavigation)
                .OrderByDescending(hd => hd.NgayToChuc)
                .Take(9)
                .ToListAsync();

            viewModel.DanhSachHoatDong = danhSachHoatDongDb
                .Select(hd => new HoatDongViewModel
                {
                    MaHD = hd.MaHd,
                    TenHD = hd.TenHd,
                    NgayToChuc = hd.NgayToChuc.HasValue
                        ? hd.NgayToChuc.Value.ToDateTime(TimeOnly.MinValue)
                        : null,
                    DiaDiem = hd.DiaDiem,
                    MoTa = hd.MoTa,
                    TenLoaiHD = hd.MaLoaiHdNavigation?.TenLoaiHd ?? "",
                    TrangThai = hd.TrangThai,
                    HinhAnh = "https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=800"
                })
                .ToList();

            // 5. Danh sách khen thưởng
            var danhSachKhenThuongDb = await _context.KhenThuongs
                .Where(kt => kt.MaTv == maTV)
                .OrderByDescending(kt => kt.NgayKt)
                .Take(5)
                .ToListAsync();

            viewModel.DanhSachKhenThuong = danhSachKhenThuongDb
                .Select(kt => new KhenThuongViewModel
                {
                    MaKT = kt.MaKt,
                    LyDo = kt.LyDo,
                    NgayKT = kt.NgayKt.HasValue
                        ? kt.NgayKt.Value.ToDateTime(TimeOnly.MinValue)
                        : DateTime.MinValue, // hoặc để null nếu bạn muốn
                    LoaiKhenThuong = "Gold"
                })
                .ToList();

            // 6. Timeline (Hoạt động + Lịch họp sắp tới)
            var hoatDongTimeline = await _context.HoatDongs
                .OrderBy(hd => hd.NgayToChuc)
                .Select(hd => new TimelineItemViewModel
                {
                    Loai = "HoatDong",
                    MaItem = hd.MaHd,
                    TieuDe = hd.TenHd,
                    NoiDung = hd.MoTa,
                    NgayDienRa = hd.NgayToChuc.HasValue
                        ? hd.NgayToChuc.Value.ToDateTime(TimeOnly.MinValue)
                        : (DateTime?)null,
                    DiaDiem = hd.DiaDiem,
                    MucDoUuTien = "Trung bình",
                    Icon = "💻"
                })
                .ToListAsync();


            var lichHopTimeline = await _context.LichHops
                .Where(lh => lh.NgayHop >= DateTime.Now.Date)
                .OrderBy(lh => lh.NgayHop)
                .Take(3)
                .Select(lh => new TimelineItemViewModel
                {
                    Loai = "LichHop",
                    MaItem = lh.MaLh,
                    TieuDe = "Lịch họp",
                    NoiDung = lh.NoiDung,
                    NgayDienRa = lh.NgayHop, // Không cần HasValue
                    DiaDiem = lh.DiaDiem,
                    MucDoUuTien = "Cao",
                    Icon = "🚨"
                })
                .ToListAsync();


            viewModel.DanhSachTimeline = hoatDongTimeline
                .Concat(lichHopTimeline)
                .OrderBy(t => t.NgayDienRa)
                .Take(6)
                .ToList();

            return View(viewModel);
        }
    }
}