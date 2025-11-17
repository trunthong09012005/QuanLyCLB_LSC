using System.Collections.Generic;
using QuanLyCLB_LSC.Models;

namespace QuanLyCLB_LSC.ViewModels
{
    public class UserDashboardViewModel
    {
        public ThanhVien ThanhVien { get; set; }
        public List<HoatDong> HoatDongs { get; set; }
        public List<DuAn> DuAns { get; set; }
        public List<ThuChi> ThuChis { get; set; }
        public List<LichHop> LichHops { get; set; }
        public List<ThongBao> ThongBaos { get; set; }
        public List<TinNhan> TinNhans { get; set; }
    }
}
