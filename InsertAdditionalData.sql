USE QL_CLB_LSC;
GO

-- Insert additional sample data for reports (separate file)
-- NOTE: Only insert ThanhVien records here (no TaiKhoan).
-- Diverse sample data: at least 10 members, activities, projects, finance, reports.

SET NOCOUNT ON;

-- 1) Thêm t?i thi?u 10 thành viên m?u (KHÔNG t?o tài kho?n ? ?ây)
INSERT INTO ThanhVien (HoTen, NgaySinh, GioiTinh, Lop, Khoa, SDT, Email, DiaChi, VaiTro, MaCV, MaBan)
VALUES
(N'Ph?m V?n A', '2003-03-12', N'Nam', N'DHKTPM18A', N'Công ngh? thông tin', '0901111222', N'pva@example.com', N'Qu?n 1, TP.HCM', N'Thành viên', NULL, NULL),
(N'Nguy?n Th? B', '2004-06-05', N'N?', N'DHKTPM18B', N'Khoa h?c máy tính', '0902222333', N'ntb@example.com', N'Qu?n 3, TP.HCM', N'Thành viên', NULL, NULL),
(N'Lê V?n C', '2002-11-20', N'Nam', N'DHKTPM18C', N'Khoa h?c máy tính', '0903333444', N'lvc@example.com', N'Qu?n 7, TP.HCM', N'Thành viên', NULL, NULL),
(N'Tr?n Th? D', '2003-08-30', N'N?', N'DHKTPM19A', N'CNTT', '0904444555', N'ttd@example.com', N'Qu?n 5, TP.HCM', N'Thành viên', NULL, NULL),
(N'Hoàng V?n E', '2001-12-01', N'Nam', N'DHKTPM19B', N'Khoa h?c máy tính', '0905555666', N'hve@example.com', N'Qu?n 2, TP.HCM', N'Thành viên', NULL, NULL),
(N'Ph??ng Thùy F', '2004-02-17', N'N?', N'DHKTPM20A', N'CNTT', '0906666777', N'ptf@example.com', N'Qu?n 4, TP.HCM', N'Thành viên', NULL, NULL),
(N'Ngô V?n G', '2002-05-09', N'Nam', N'DHKTPM20B', N'Khoa h?c máy tính', '0907777888', N'ngv@example.com', N'Qu?n 6, TP.HCM', N'Thành viên', NULL, NULL),
(N'??ng Th? H', '2003-10-22', N'N?', N'DHKTPM21A', N'CNTT', '0908888999', N'dth@example.com', N'Qu?n 8, TP.HCM', N'Thành viên', NULL, NULL),
(N'V? V?n I', '2000-07-14', N'Nam', N'DHKTPM21B', N'Khoa h?c máy tính', '0909999000', N'vvi@example.com', N'Qu?n 9, TP.HCM', N'Thành viên', NULL, NULL),
(N'Bùi Th? K', '2002-09-05', N'N?', N'DHKTPM22A', N'CNTT', '0910000111', N'btK@example.com', N'Qu?n 10, TP.HCM', N'Thành viên', NULL, NULL);

-- 2) Thêm vài Ho?t ??ng m?u (?a d?ng tháng/n?m, dùng MaTV tham chi?u theo Email)
INSERT INTO HoatDong (TenHD, NgayToChuc, DiaDiem, MoTa, MaLoaiHD, NguoiPhuTrach, KinhPhiDuKien, TrangThai)
VALUES
(N'Bu?i ?ào t?o K? n?ng m?m', '2025-01-15', N'H?i tr??ng A', N'?ào t?o giao ti?p và thuy?t trình', NULL, (SELECT MaTV FROM ThanhVien WHERE Email = N'pva@example.com'), 1000000, N'Hoàn thành'),
(N'Sinh ho?t CLB tháng 3', '2025-03-10', N'Phòng B', N'Sinh ho?t t?ng k?t tháng 3', NULL, (SELECT MaTV FROM ThanhVien WHERE Email = N'ntb@example.com'), 500000, N'Hoàn thành'),
(N'Chi?n d?ch tình nguy?n', '2025-05-20', N'Qu?n 9', N'Ho?t ??ng c?ng ??ng', NULL, (SELECT MaTV FROM ThanhVien WHERE Email = N'lvc@example.com'), 2000000, N'Hoàn thành'),
(N'H?i th?o CNTT', '2025-08-12', N'H?i tr??ng C', N'Chia s? công ngh? m?i', NULL, (SELECT MaTV FROM ThanhVien WHERE Email = N'ttd@example.com'), 1500000, N'?ã t? ch?c'),
(N'Workshop thi?t k?', '2025-02-25', N'Phòng Th?c hành', N'Gi?ng d?y Canva và Photoshop', NULL, (SELECT MaTV FROM ThanhVien WHERE Email = N'hve@example.com'), 800000, N'Hoàn thành'),
(N'Tr?i hè k? n?ng', '2024-07-05', N'Dã ngo?i', N'Rèn luy?n k? n?ng s?ng', NULL, (SELECT MaTV FROM ThanhVien WHERE Email = N'ptf@example.com'), 3000000, N'Hoàn thành');

-- 3) Thêm vài D? Án m?u
INSERT INTO DuAn (TenDuAn, MoTa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES
(N'?ng d?ng qu?n lý s? ki?n', N'Xây d?ng ?ng d?ng qu?n lý s? ki?n n?i b?', '2025-02-01', '2025-06-30', N'Hoàn thành'),
(N'H? th?ng báo cáo n?i b?', N'Tri?n khai h? th?ng báo cáo', '2025-04-01', NULL, N'?ang th?c hi?n'),
(N'Website gi?i thi?u CLB', N'Thi?t k? website thông tin', '2024-11-01', '2025-03-30', N'Hoàn thành');

-- 4) Thêm Thu/Chi m?u r?i theo tháng (?a d?ng ngu?n/lo?i)
INSERT INTO ThuChi (LoaiGD, SoTien, NgayGD, NoiDung, NguoiThucHien, MaNguon)
VALUES
(N'Thu', 5000000, '2025-01-10', N'Tài tr? công ty ABC', (SELECT MaTV FROM ThanhVien WHERE Email = N'pva@example.com'), NULL),
(N'Chi', 1200000, '2025-01-12', N'Chi phí h?u c?n', (SELECT MaTV FROM ThanhVien WHERE Email = N'ntb@example.com'), NULL),
(N'Thu', 3000000, '2025-03-15', N'?óng góp thành viên', (SELECT MaTV FROM ThanhVien WHERE Email = N'lvc@example.com'), NULL),
(N'Chi', 800000, '2025-05-22', N'Mua v?t t?', (SELECT MaTV FROM ThanhVien WHERE Email = N'pva@example.com'), NULL),
(N'Chi', 450000, '2025-02-18', N'Thuê ??a ?i?m', (SELECT MaTV FROM ThanhVien WHERE Email = N'ttd@example.com'), NULL),
(N'Thu', 1500000, '2024-12-05', N'Qu? ho?t ??ng', (SELECT MaTV FROM ThanhVien WHERE Email = N'hve@example.com'), NULL);

-- 5) Thêm Báo Cáo m?u (liên quan t?i thành viên, ho?t ??ng, tài chính)
INSERT INTO BaoCao (TieuDe, LoaiBC, NoiDung, NgayLap, NguoiLap)
VALUES
(N'Báo cáo ho?t ??ng tháng 1', N'Ho?t ??ng', N'Báo cáo t?ng k?t ho?t ??ng tháng 1', '2025-01-31', (SELECT MaTV FROM ThanhVien WHERE Email = N'pva@example.com')),
(N'Báo cáo thành viên Q1', N'Thành viên', N'Th?ng kê thành viên quý 1', '2025-03-31', (SELECT MaTV FROM ThanhVien WHERE Email = N'ntb@example.com')),
(N'Báo cáo tài chính gi?a k?', N'Tài chính', N'T?ng h?p thu chi gi?a k?', '2025-05-31', (SELECT MaTV FROM ThanhVien WHERE Email = N'lvc@example.com')),
(N'Báo cáo d? án', N'D? án', N'T?ng h?p ti?n ?? d? án', '2025-04-15', (SELECT MaTV FROM ThanhVien WHERE Email = N'hve@example.com'));

-- 6) Phân công m?u cho d? án (s? d?ng MaTV t? email và MaDA t? TenDuAn)
INSERT INTO PhanCong (MaTV, MaDA, NhiemVu, TrangThai)
SELECT tv.MaTV, da.MaDA, N'Ph? trách module', N'?ang th?c hi?n'
FROM (SELECT MaTV FROM ThanhVien WHERE Email = N'pva@example.com') tv
CROSS JOIN (SELECT MaDA FROM DuAn WHERE TenDuAn = N'H? th?ng báo cáo n?i b?') da;

-- 7) Tham gia ho?t ??ng (ThamGia ho?c ThamGium) n?u b?ng t?n t?i
IF OBJECT_ID('ThamGia', 'U') IS NOT NULL
BEGIN
    INSERT INTO ThamGia (MaHD, MaTV, VaiTroTrongHD, DiemDanh)
    SELECT hd.MaHD, tv.MaTV, N'Ng??i tham gia', 1
    FROM HoatDong hd
    JOIN ThanhVien tv ON tv.Email = N'pva@example.com'
    WHERE hd.TenHD = N'Bu?i ?ào t?o K? n?ng m?m';
END
ELSE IF OBJECT_ID('ThamGium', 'U') IS NOT NULL
BEGIN
    INSERT INTO ThamGium (MaHD, MaTV, VaiTroTrongHD, DiemDanh)
    SELECT hd.MaHD, tv.MaTV, N'Ng??i tham gia', 1
    FROM HoatDong hd
    JOIN ThanhVien tv ON tv.Email = N'pva@example.com'
    WHERE hd.TenHD = N'Bu?i ?ào t?o K? n?ng m?m';
END

PRINT 'InsertAdditionalData.sql completed: added members (>=10), activities, projects, finance, reports.';
GO
