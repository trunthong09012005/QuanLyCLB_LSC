
DROP DATABASE IF EXISTS QL_CLB_LSC;
GO
CREATE DATABASE QL_CLB_LSC;
GO
USE QL_CLB_LSC;

-- =============================================
-- BẢNG LOOKUP / THAM CHIẾU
-- =============================================

CREATE TABLE ChucVu (
    MaCV INT IDENTITY(1,1) PRIMARY KEY,
    TenCV NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL
);
GO

CREATE TABLE BanChuyenMon (
    MaBan INT IDENTITY(1,1) PRIMARY KEY,
    TenBan NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL,
    TruongBan INT NULL -- tham chiếu tới ThanhVien.MaTV (tạo sau)
);
GO

CREATE TABLE LoaiHoatDong (
    MaLoaiHD INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiHD NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL
);
GO

CREATE TABLE NguonThu (
    MaNguon INT IDENTITY(1,1) PRIMARY KEY,
    TenNguon NVARCHAR(150) NOT NULL,
    MoTa NVARCHAR(255) NULL
);
GO

CREATE TABLE KyNang (
    MaKN INT IDENTITY(1,1) PRIMARY KEY,
    TenKN NVARCHAR(150) NOT NULL,
    MoTa NVARCHAR(255) NULL,
    CapDo NVARCHAR(50) NULL
);
GO

-- =============================================
-- BẢNG CHÍNH
-- =============================================

CREATE TABLE ThanhVien (
    MaTV INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(150) NOT NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(10) CHECK (GioiTinh IN (N'Nam', N'Nữ', N'Khác')),
    Lop NVARCHAR(50) NULL,
    Khoa NVARCHAR(150) NULL,
    SDT VARCHAR(15) NULL,
    Email NVARCHAR(150) NULL,
    DiaChi NVARCHAR(255) NULL,
    VaiTro NVARCHAR(100) NULL,
    NgayThamGia DATE DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động',
    MaCV INT NULL,      -- FK -> ChucVu
    MaBan INT NULL      -- FK -> BanChuyenMon (tùy trước vẫn có)
);
GO

ALTER TABLE ThanhVien
ADD CONSTRAINT UQ_ThanhVien_Email UNIQUE (Email);
GO

ALTER TABLE ThanhVien
ADD CONSTRAINT FK_ThanhVien_ChucVu FOREIGN KEY (MaCV)
REFERENCES ChucVu(MaCV);
GO

-- BanChuyenMon.TruongBan tham chiếu ThanhVien (bởi vì ThanhVien tạo sau BanChuyenMon),
-- ta thêm FK sau khi ThanhVien có rồi:
ALTER TABLE BanChuyenMon
ADD CONSTRAINT FK_BanTruong_ThanhVien FOREIGN KEY (TruongBan)
REFERENCES ThanhVien(MaTV);
GO

ALTER TABLE ThanhVien
ADD CONSTRAINT FK_ThanhVien_Ban FOREIGN KEY (MaBan)
REFERENCES BanChuyenMon(MaBan);
GO

-- =============================================
-- TÀI KHOẢN
-- =============================================
CREATE TABLE TaiKhoan (
    MaTK INT IDENTITY(1,1) PRIMARY KEY,
    TenDN NVARCHAR(50) NOT NULL UNIQUE,
    MatKhau NVARCHAR(255) NOT NULL,
    MaTV INT NOT NULL UNIQUE, -- 1-1 với ThanhVien
    QuyenHan NVARCHAR(50) NOT NULL DEFAULT N'Thành viên',
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động'
);
GO

ALTER TABLE TaiKhoan
ADD CONSTRAINT FK_TaiKhoan_ThanhVien FOREIGN KEY (MaTV)
REFERENCES ThanhVien(MaTV);
GO

-- =============================================
-- BAN <-> THANHVIEN (N-N)
-- =============================================
CREATE TABLE BanChuyenMon_ThanhVien (
    MaBan INT NOT NULL,
    MaTV INT NOT NULL,
    VaiTro NVARCHAR(100) NULL,
    NgayThamGiaBan DATE DEFAULT GETDATE(),
    PRIMARY KEY (MaBan, MaTV),
    CONSTRAINT FK_BCTV_Ban FOREIGN KEY (MaBan) REFERENCES BanChuyenMon(MaBan) ON DELETE CASCADE,
    CONSTRAINT FK_BCTV_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV) ON DELETE CASCADE
);
GO

-- =============================================
-- KỸ NĂNG: ThanhVien_KyNang (N-N)
-- =============================================
CREATE TABLE ThanhVien_KyNang (
    MaTV INT NOT NULL,
    MaKN INT NOT NULL,
    Diem FLOAT DEFAULT 0,
    CapDoHienTai NVARCHAR(50) NULL,
    NgayCapNhat DATE DEFAULT GETDATE(),
    PRIMARY KEY (MaTV, MaKN),
    CONSTRAINT FK_TVKN_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV) ON DELETE CASCADE,
    CONSTRAINT FK_TVKN_KN FOREIGN KEY (MaKN) REFERENCES KyNang(MaKN) ON DELETE CASCADE
);
GO

-- =============================================
-- HOẠT ĐỘNG, ĐĂNG KÝ, THAM GIA
-- =============================================
CREATE TABLE HoatDong (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    TenHD NVARCHAR(200) NOT NULL,
    NgayToChuc DATE NULL,
    DiaDiem NVARCHAR(255) NULL,
    MoTa NVARCHAR(500) NULL,
    MaLoaiHD INT NULL,
    NguoiPhuTrach INT NULL,  -- MaTV
    KinhPhiDuKien MONEY NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Đang chuẩn bị'
);
GO

ALTER TABLE HoatDong
ADD CONSTRAINT FK_HoatDong_Loai FOREIGN KEY (MaLoaiHD) REFERENCES LoaiHoatDong(MaLoaiHD);
GO

ALTER TABLE HoatDong
ADD CONSTRAINT FK_HoatDong_TV FOREIGN KEY (NguoiPhuTrach) REFERENCES ThanhVien(MaTV);
GO

CREATE TABLE DangKyHoatDong (
    MaTV INT NOT NULL,
    MaHD INT NOT NULL,
    ThoiGianDangKy DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT N'Chờ duyệt',
    PRIMARY KEY (MaTV, MaHD),
    CONSTRAINT FK_DK_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV) ON DELETE CASCADE,
    CONSTRAINT FK_DK_HD FOREIGN KEY (MaHD) REFERENCES HoatDong(MaHD) ON DELETE CASCADE
);
GO

CREATE TABLE ThamGia (
    MaHD INT NOT NULL,
    MaTV INT NOT NULL,
    VaiTroTrongHD NVARCHAR(100) NULL,
    DiemDanh BIT DEFAULT 0,
    DiemThuong FLOAT DEFAULT 0,
    DanhGia NVARCHAR(500) NULL,
    GhiChu NVARCHAR(500) NULL,
    PRIMARY KEY (MaHD, MaTV),
    CONSTRAINT FK_ThamGia_HD FOREIGN KEY (MaHD) REFERENCES HoatDong(MaHD) ON DELETE CASCADE,
    CONSTRAINT FK_ThamGia_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV) ON DELETE CASCADE
);
GO

-- =============================================
-- DỰ ÁN & PHÂN CÔNG
-- =============================================
CREATE TABLE DuAn (
    MaDA INT IDENTITY(1,1) PRIMARY KEY,
    TenDuAn NVARCHAR(200) NOT NULL,
    MoTa NVARCHAR(500) NULL,
    NgayBatDau DATE NULL,
    NgayKetThuc DATE NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Đang thực hiện'
);
GO

CREATE TABLE PhanCong (
    MaTV INT NOT NULL,
    MaDA INT NOT NULL,
    NhiemVu NVARCHAR(500) NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Chưa hoàn thành',
    NgayPhanCong DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (MaTV, MaDA),
    CONSTRAINT FK_PhanCong_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV) ON DELETE CASCADE,
    CONSTRAINT FK_PhanCong_DA FOREIGN KEY (MaDA) REFERENCES DuAn(MaDA) ON DELETE CASCADE
);
GO

-- =============================================
-- TÀI CHÍNH (ThuChi + chi tiết)
-- =============================================
CREATE TABLE ThuChi (
    MaGD INT IDENTITY(1,1) PRIMARY KEY,
    LoaiGD NVARCHAR(10) CHECK (LoaiGD IN (N'Thu', N'Chi')),
    SoTien MONEY NOT NULL,
    NgayGD DATE DEFAULT GETDATE(),
    NoiDung NVARCHAR(500) NULL,
    NguoiThucHien INT NULL,  -- MaTV
    MaHD INT NULL,
    MaNguon INT NULL,
    CONSTRAINT FK_ThuChi_TV FOREIGN KEY (NguoiThucHien) REFERENCES ThanhVien(MaTV),
    CONSTRAINT FK_ThuChi_HD FOREIGN KEY (MaHD) REFERENCES HoatDong(MaHD),
    CONSTRAINT FK_ThuChi_Nguon FOREIGN KEY (MaNguon) REFERENCES NguonThu(MaNguon)

);
GO

CREATE TABLE ThuChi_ChiTiet (
    MaCT INT IDENTITY(1,1) PRIMARY KEY,
    MaGD INT NOT NULL,
    NoiDung NVARCHAR(500) NULL,
    SoTien DECIMAL(18,2) NOT NULL CHECK (SoTien >= 0),
    CONSTRAINT FK_ThuChiCT_ThuChi FOREIGN KEY (MaGD) REFERENCES ThuChi(MaGD) ON DELETE CASCADE
);
GO

-- =============================================
-- TÀI SẢN
-- =============================================
CREATE TABLE TaiSan (
    MaTS INT IDENTITY(1,1) PRIMARY KEY,
    TenTS NVARCHAR(255),
    SoLuong INT CHECK (SoLuong >= 0),
    DonViTinh NVARCHAR(50),
    TinhTrang NVARCHAR(100) DEFAULT N'Tốt',
    NgayNhap DATE DEFAULT GETDATE(),
    NguoiQuanLy INT,
    GhiChu NVARCHAR(255),
    FOREIGN KEY (NguoiQuanLy) REFERENCES ThanhVien(MaTV)
);


-- =============================================
-- LỊCH HỌP & ĐIỂM DANH
-- =============================================
CREATE TABLE LichHop (
    MaLH INT IDENTITY(1,1) PRIMARY KEY,
    NgayHop DATETIME NOT NULL,
    DiaDiem NVARCHAR(255) NULL,
    NoiDung NVARCHAR(500) NULL,
    NguoiChuTri INT NULL, -- MaTV
    GhiChu NVARCHAR(500) NULL,
    CONSTRAINT FK_LichHop_TV FOREIGN KEY (NguoiChuTri) REFERENCES ThanhVien(MaTV)
);
GO

CREATE TABLE DiemDanhLichHop (
    MaLH INT NOT NULL,
    MaTV INT NOT NULL,
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Có mặt', N'Vắng', N'Trễ')) DEFAULT N'Có mặt',
    PRIMARY KEY (MaLH, MaTV),
    CONSTRAINT FK_DDLH_LH FOREIGN KEY (MaLH) REFERENCES LichHop(MaLH) ON DELETE CASCADE,
    CONSTRAINT FK_DDLH_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV) ON DELETE CASCADE
);
GO

-- =============================================
-- KHEN THƯỞNG & KỶ LUẬT (tách riêng)
-- =============================================
CREATE TABLE KhenThuong (
    MaKT INT IDENTITY(1,1) PRIMARY KEY,
    MaTV INT NOT NULL,
    LyDo NVARCHAR(500) NOT NULL,
    NgayKT DATE DEFAULT GETDATE(),
    NguoiLap INT NULL,
    CONSTRAINT FK_KhenThuong_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV)
);
GO

CREATE TABLE KyLuat (
    MaKL INT IDENTITY(1,1) PRIMARY KEY,
    MaTV INT NOT NULL,
    LyDo NVARCHAR(500) NOT NULL,
    NgayKL DATE DEFAULT GETDATE(),
    NguoiLap INT NULL,
    CONSTRAINT FK_KyLuat_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV)
);
GO

-- =============================================
-- FEEDBACK, FILES, BÁO CÁO, ĐIỂM RÈN LUYỆN
-- =============================================
CREATE TABLE Feedback (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    MaTV INT NOT NULL,
    MaHD INT NULL,
    NoiDung NVARCHAR(1000) NOT NULL,
    NgayGopY DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT N'Đã nhận',
    CONSTRAINT FK_Feedback_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV),
    CONSTRAINT FK_Feedback_HD FOREIGN KEY (MaHD) REFERENCES HoatDong(MaHD)
);
GO

CREATE TABLE FileDinhKem (
    MaFile INT IDENTITY(1,1) PRIMARY KEY,
    TenFile NVARCHAR(255) NOT NULL,
    DuongDan NVARCHAR(500) NOT NULL,
    LoaiFile NVARCHAR(50) NULL,
    MaDA INT NULL,
    MaHD INT NULL,
    CONSTRAINT FK_File_DA FOREIGN KEY (MaDA) REFERENCES DuAn(MaDA),
    CONSTRAINT FK_File_HD FOREIGN KEY (MaHD) REFERENCES HoatDong(MaHD)
);
GO

CREATE TABLE DiemRenLuyen (
    MaDRL INT IDENTITY(1,1) PRIMARY KEY,
    MaTV INT NOT NULL,
    HocKy NVARCHAR(20) NULL,
    NamHoc NVARCHAR(20) NULL,
    Diem INT CHECK (Diem BETWEEN 0 AND 100),
    CONSTRAINT FK_DRl_TV FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV)
);
GO

CREATE TABLE BaoCao (
    MaBC INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe NVARCHAR(255) NOT NULL,
    LoaiBC NVARCHAR(100) NULL,
    NoiDung NVARCHAR(MAX) NULL,
    NgayLap DATETIME DEFAULT GETDATE(),
    NguoiLap INT NULL,
    CONSTRAINT FK_BaoCao_TV FOREIGN KEY (NguoiLap) REFERENCES ThanhVien(MaTV)
);
GO

-- =============================================
-- BẢNG LƯU LỊCH SỬ (AUDIT)
-- =============================================
drop table LichSuThaoTac;
CREATE TABLE LichSuThaoTac (
    MaLSTT INT IDENTITY(1,1) PRIMARY KEY,
    MaTV INT NULL,
    TenBang NVARCHAR(100),
    LoaiThaoTac NVARCHAR(50),
    KhoaChinh NVARCHAR(100),
    NoiDung NVARCHAR(500),
    NgayThucHien DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaTV) REFERENCES ThanhVien(MaTV)
);




CREATE TABLE ThongBao (
    MaTB INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe NVARCHAR(255) NOT NULL,
    NoiDung NVARCHAR(MAX),
    NgayDang DATETIME DEFAULT GETDATE(),
    NguoiDang INT,
    DoiTuong NVARCHAR(100) DEFAULT N'Tất cả', -- hoặc 'Ban Truyền thông', 'Thành viên mới'
    FOREIGN KEY (NguoiDang) REFERENCES ThanhVien(MaTV)

);
	ALTER TABLE ThongBao ADD NgayGui DATETIME DEFAULT GETDATE();


CREATE TABLE TinNhan (
    MaTN INT IDENTITY(1,1) PRIMARY KEY,
    MaNguoiGui INT NOT NULL,
    MaNguoiNhan INT NOT NULL,
    NoiDung NVARCHAR(500) NOT NULL,
    NgayGui DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT N'Chưa đọc',
    FOREIGN KEY (MaNguoiGui) REFERENCES ThanhVien(MaTV),
    FOREIGN KEY (MaNguoiNhan) REFERENCES ThanhVien(MaTV)

);

-- 1️⃣ Ràng buộc: Số tiền trong ThuChi phải > 0
ALTER TABLE ThuChi
ADD CONSTRAINT CK_ThuChi_SoTien CHECK (SoTien > 0);
GO

-- 2️⃣ Ràng buộc: Độ dài số điện thoại từ 9 đến 15 ký tự
ALTER TABLE ThanhVien
ADD CONSTRAINT CK_ThanhVien_SDT CHECK (LEN(SDT) BETWEEN 9 AND 15);
GO

-- 3️⃣ Ràng buộc: Email phải có ký tự '@' và '.'
ALTER TABLE ThanhVien
ADD CONSTRAINT CK_ThanhVien_Email_Format CHECK (Email LIKE '[A-Za-z0-9._%+-]%@%.[A-Za-z]%')

GO

