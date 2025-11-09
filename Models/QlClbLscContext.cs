using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

public partial class QlClbLscContext : DbContext
{
    public QlClbLscContext()
    {
    }

    public QlClbLscContext(DbContextOptions<QlClbLscContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BanChuyenMon> BanChuyenMons { get; set; }

    public virtual DbSet<BanChuyenMonThanhVien> BanChuyenMonThanhViens { get; set; }

    public virtual DbSet<BaoCao> BaoCaos { get; set; }

    public virtual DbSet<ChucVu> ChucVus { get; set; }

    public virtual DbSet<DangKyHoatDong> DangKyHoatDongs { get; set; }

    public virtual DbSet<DiemDanhLichHop> DiemDanhLichHops { get; set; }

    public virtual DbSet<DiemRenLuyen> DiemRenLuyens { get; set; }

    public virtual DbSet<DuAn> DuAns { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FileDinhKem> FileDinhKems { get; set; }

    public virtual DbSet<HoatDong> HoatDongs { get; set; }

    public virtual DbSet<HoatDongThanhVien> HoatDongThanhViens { get; set; }

    public virtual DbSet<KhenThuong> KhenThuongs { get; set; }

    public virtual DbSet<KyLuat> KyLuats { get; set; }

    public virtual DbSet<KyNang> KyNangs { get; set; }

    public virtual DbSet<LichHop> LichHops { get; set; }

    public virtual DbSet<LichSuThaoTac> LichSuThaoTacs { get; set; }

    public virtual DbSet<LoaiHoatDong> LoaiHoatDongs { get; set; }

    public virtual DbSet<NguonThu> NguonThus { get; set; }

    public virtual DbSet<PhanCong> PhanCongs { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<TaiSan> TaiSans { get; set; }

    public virtual DbSet<ThamGium> ThamGia { get; set; }

    public virtual DbSet<ThanhVien> ThanhViens { get; set; }

    public virtual DbSet<ThanhVienKyNang> ThanhVienKyNangs { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<ThuChi> ThuChis { get; set; }

    public virtual DbSet<ThuChiChiTiet> ThuChiChiTiets { get; set; }

    public virtual DbSet<TinNhan> TinNhans { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    public virtual DbSet<VwDangKyChoDuyet> VwDangKyChoDuyets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BanChuyenMon>(entity =>
        {
            entity.HasKey(e => e.MaBan).HasName("PK__BanChuye__3520ED6C5A57D136");

            entity.HasOne(d => d.TruongBanNavigation).WithMany(p => p.BanChuyenMons).HasConstraintName("FK_BanTruong_ThanhVien");
        });

        modelBuilder.Entity<BanChuyenMonThanhVien>(entity =>
        {
            entity.HasKey(e => new { e.MaBan, e.MaTv }).HasName("PK__BanChuye__9752BD6BC1BE9E52");

            entity.ToTable("BanChuyenMon_ThanhVien", tb => tb.HasTrigger("TRG_BanChuyenMon_ThanhVien_Audit"));

            entity.Property(e => e.NgayThamGiaBan).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaBanNavigation).WithMany(p => p.BanChuyenMonThanhViens).HasConstraintName("FK_BCTV_Ban");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.BanChuyenMonThanhViens).HasConstraintName("FK_BCTV_TV");
        });

        modelBuilder.Entity<BaoCao>(entity =>
        {
            entity.HasKey(e => e.MaBc).HasName("PK__BaoCao__272475A6F53853F4");

            entity.Property(e => e.NgayLap).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.NguoiLapNavigation).WithMany(p => p.BaoCaos).HasConstraintName("FK_BaoCao_TV");
        });

        modelBuilder.Entity<ChucVu>(entity =>
        {
            entity.HasKey(e => e.MaCv).HasName("PK__ChucVu__27258E769515D065");
        });

        modelBuilder.Entity<DangKyHoatDong>(entity =>
        {
            entity.HasKey(e => new { e.MaTv, e.MaHd }).HasName("PK__DangKyHo__35575A15C5D8C9A3");

            entity.ToTable("DangKyHoatDong", tb => tb.HasTrigger("TRG_DangKyHoatDong_Audit"));

            entity.Property(e => e.ThoiGianDangKy).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue("Chờ duyệt");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.DangKyHoatDongs).HasConstraintName("FK_DK_HD");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.DangKyHoatDongs).HasConstraintName("FK_DK_TV");
        });

        modelBuilder.Entity<DiemDanhLichHop>(entity =>
        {
            entity.HasKey(e => new { e.MaLh, e.MaTv }).HasName("PK__DiemDanh__85579778BBBB49D0");

            entity.Property(e => e.TrangThai).HasDefaultValue("Có mặt");

            entity.HasOne(d => d.MaLhNavigation).WithMany(p => p.DiemDanhLichHops).HasConstraintName("FK_DDLH_LH");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.DiemDanhLichHops).HasConstraintName("FK_DDLH_TV");
        });

        modelBuilder.Entity<DiemRenLuyen>(entity =>
        {
            entity.HasKey(e => e.MaDrl).HasName("PK__DiemRenL__3D88F94D8FEDD0EB");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.DiemRenLuyens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DRl_TV");
        });

        modelBuilder.Entity<DuAn>(entity =>
        {
            entity.HasKey(e => e.MaDa).HasName("PK__DuAn__2725867A921CCCE3");

            entity.Property(e => e.TrangThai).HasDefaultValue("Đang thực hiện");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC27E2ADBFDB");

            entity.ToTable("Feedback", tb => tb.HasTrigger("TRG_Feedback_Audit"));

            entity.Property(e => e.NgayGopY).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue("Đã nhận");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.Feedbacks).HasConstraintName("FK_Feedback_HD");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.Feedbacks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feedback_TV");
        });

        modelBuilder.Entity<FileDinhKem>(entity =>
        {
            entity.HasKey(e => e.MaFile).HasName("PK__FileDinh__E2361D457BB64D76");

            entity.HasOne(d => d.MaDaNavigation).WithMany(p => p.FileDinhKems).HasConstraintName("FK_File_DA");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.FileDinhKems).HasConstraintName("FK_File_HD");
        });

        modelBuilder.Entity<HoatDong>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoatDong__2725A6E0DDC25EB7");

            entity.ToTable("HoatDong", tb => tb.HasTrigger("TRG_HoatDong_Audit"));

            entity.Property(e => e.TrangThai).HasDefaultValue("Đang chuẩn bị");

            entity.HasOne(d => d.MaLoaiHdNavigation).WithMany(p => p.HoatDongs).HasConstraintName("FK_HoatDong_Loai");

            entity.HasOne(d => d.NguoiPhuTrachNavigation).WithMany(p => p.HoatDongs).HasConstraintName("FK_HoatDong_TV");
        });

        modelBuilder.Entity<HoatDongThanhVien>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaTv }).HasName("PK__HoatDong__8557F6E7DC72FCF9");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.HoatDongThanhViens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoatDong_T__MaHD__7C1A6C5A");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.HoatDongThanhViens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoatDong_T__MaTV__7D0E9093");
        });

        modelBuilder.Entity<KhenThuong>(entity =>
        {
            entity.HasKey(e => e.MaKt).HasName("PK__KhenThuo__2725CF12F2DFFCAD");

            entity.ToTable("KhenThuong", tb => tb.HasTrigger("TRG_KhenThuong_Audit"));

            entity.Property(e => e.NgayKt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.KhenThuongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhenThuong_TV");
        });

        modelBuilder.Entity<KyLuat>(entity =>
        {
            entity.HasKey(e => e.MaKl).HasName("PK__KyLuat__2725CF1AC13D6DB1");

            entity.ToTable("KyLuat", tb => tb.HasTrigger("TRG_KyLuat_Audit"));

            entity.Property(e => e.NgayKl).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.KyLuats)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KyLuat_TV");
        });

        modelBuilder.Entity<KyNang>(entity =>
        {
            entity.HasKey(e => e.MaKn).HasName("PK__KyNang__2725CF14D94857C5");
        });

        modelBuilder.Entity<LichHop>(entity =>
        {
            entity.HasKey(e => e.MaLh).HasName("PK__LichHop__2725C77F93E0C7B1");

            entity.HasOne(d => d.NguoiChuTriNavigation).WithMany(p => p.LichHops).HasConstraintName("FK_LichHop_TV");
        });

        modelBuilder.Entity<LichSuThaoTac>(entity =>
        {
            entity.HasKey(e => e.MaLstt).HasName("PK__LichSuTh__78751B4EBA7FA03D");

            entity.Property(e => e.NgayThucHien).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.LichSuThaoTacs).HasConstraintName("FK__LichSuThao__MaTV__0A688BB1");
        });

        modelBuilder.Entity<LoaiHoatDong>(entity =>
        {
            entity.HasKey(e => e.MaLoaiHd).HasName("PK__LoaiHoat__122768D28F5D55A0");
        });

        modelBuilder.Entity<NguonThu>(entity =>
        {
            entity.HasKey(e => e.MaNguon).HasName("PK__NguonThu__ABEE19F8DD409A72");
        });

        modelBuilder.Entity<PhanCong>(entity =>
        {
            entity.HasKey(e => new { e.MaTv, e.MaDa }).HasName("PK__PhanCong__9557581CCD7FB75D");

            entity.Property(e => e.NgayPhanCong).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue("Chưa hoàn thành");

            entity.HasOne(d => d.MaDaNavigation).WithMany(p => p.PhanCongs).HasConstraintName("FK_PhanCong_DA");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.PhanCongs).HasConstraintName("FK_PhanCong_TV");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTk).HasName("PK__TaiKhoan__27250070308BC16F");

            entity.ToTable("TaiKhoan", tb => tb.HasTrigger("TRG_TaiKhoan_Audit"));

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.QuyenHan).HasDefaultValue("Thành viên");
            entity.Property(e => e.TrangThai).HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.MaTvNavigation).WithOne(p => p.TaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_ThanhVien");
        });

        modelBuilder.Entity<TaiSan>(entity =>
        {
            entity.HasKey(e => e.MaTs).HasName("PK__TaiSan__272500784566A62E");

            entity.Property(e => e.NgayNhap).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TinhTrang).HasDefaultValue("Tốt");

            entity.HasOne(d => d.NguoiQuanLyNavigation).WithMany(p => p.TaiSans).HasConstraintName("FK__TaiSan__NguoiQua__625A9A57");
        });

        modelBuilder.Entity<ThamGium>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaTv }).HasName("PK__ThamGia__8557F6E783EE2323");

            entity.ToTable(tb => tb.HasTrigger("TRG_ThamGia_Audit"));

            entity.Property(e => e.DiemDanh).HasDefaultValue(false);
            entity.Property(e => e.DiemThuong).HasDefaultValue(0.0);

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ThamGia).HasConstraintName("FK_ThamGia_HD");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.ThamGia).HasConstraintName("FK_ThamGia_TV");
        });

        modelBuilder.Entity<ThanhVien>(entity =>
        {
            entity.HasKey(e => e.MaTv).HasName("PK__ThanhVie__2725007BF483B8B9");

            entity.ToTable("ThanhVien", tb => tb.HasTrigger("TRG_ThanhVien_Audit"));

            entity.Property(e => e.NgayThamGia).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.MaBanNavigation).WithMany(p => p.ThanhViens).HasConstraintName("FK_ThanhVien_Ban");

            entity.HasOne(d => d.MaCvNavigation).WithMany(p => p.ThanhViens).HasConstraintName("FK_ThanhVien_ChucVu");
        });

        modelBuilder.Entity<ThanhVienKyNang>(entity =>
        {
            entity.HasKey(e => new { e.MaTv, e.MaKn }).HasName("PK__ThanhVie__75575C8A9A5FA9A8");

            entity.Property(e => e.Diem).HasDefaultValue(0.0);
            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaKnNavigation).WithMany(p => p.ThanhVienKyNangs).HasConstraintName("FK_TVKN_KN");

            entity.HasOne(d => d.MaTvNavigation).WithMany(p => p.ThanhVienKyNangs).HasConstraintName("FK_TVKN_TV");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaTb).HasName("PK__ThongBao__2725006F68E6AF4F");

            entity.Property(e => e.DoiTuong).HasDefaultValue("Tất cả");
            entity.Property(e => e.NgayDang).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayGui).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.NguoiDangNavigation).WithMany(p => p.ThongBaos).HasConstraintName("FK__ThongBao__NguoiD__3B40CD36");
        });

        modelBuilder.Entity<ThuChi>(entity =>
        {
            entity.HasKey(e => e.MaGd).HasName("PK__ThuChi__2725AE8150D56227");

            entity.ToTable("ThuChi", tb => tb.HasTrigger("TRG_ThuChi_Audit"));

            entity.Property(e => e.NgayGd).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ThuChis).HasConstraintName("FK_ThuChi_HD");

            entity.HasOne(d => d.MaNguonNavigation).WithMany(p => p.ThuChis).HasConstraintName("FK_ThuChi_Nguon");

            entity.HasOne(d => d.NguoiThucHienNavigation).WithMany(p => p.ThuChis).HasConstraintName("FK_ThuChi_TV");
        });

        modelBuilder.Entity<ThuChiChiTiet>(entity =>
        {
            entity.HasKey(e => e.MaCt).HasName("PK__ThuChi_C__27258E74D15426A9");

            entity.ToTable("ThuChi_ChiTiet", tb => tb.HasTrigger("TRG_ThuChiCT_Audit"));

            entity.HasOne(d => d.MaGdNavigation).WithMany(p => p.ThuChiChiTiets).HasConstraintName("FK_ThuChiCT_ThuChi");
        });

        modelBuilder.Entity<TinNhan>(entity =>
        {
            entity.HasKey(e => e.MaTn).HasName("PK__TinNhan__27250073B223B3E8");

            entity.Property(e => e.NgayGui).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue("Chưa đọc");

            entity.HasOne(d => d.MaNguoiGuiNavigation).WithMany(p => p.TinNhanMaNguoiGuiNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TinNhan__MaNguoi__40058253");

            entity.HasOne(d => d.MaNguoiNhanNavigation).WithMany(p => p.TinNhanMaNguoiNhanNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TinNhan__MaNguoi__40F9A68C");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CF3EFB1708");
        });

        modelBuilder.Entity<VwDangKyChoDuyet>(entity =>
        {
            entity.ToView("vw_DangKy_ChoDuyet");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
