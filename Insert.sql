USE QL_CLB_LSC;
GO

-- ================================
--  RESET TOÀN BỘ DỮ LIỆU (nếu có)
-- ================================
EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
EXEC sp_MSForEachTable 'DELETE FROM ?';
EXEC sp_MSForEachTable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
GO

-- Reset IDENTITY về 1
EXEC sp_MSForEachTable 'DBCC CHECKIDENT (''?'', RESEED, 0)';
GO


-- =============================================
-- 1️⃣ DỮ LIỆU BẢNG TRA (LOOKUP)
-- =============================================

INSERT INTO ChucVu (TenCV, MoTa)
VALUES 
(N'Chủ nhiệm', N'Lãnh đạo CLB'),
(N'Phó chủ nhiệm', N'Hỗ trợ điều hành'),
(N'Ủy viên', N'Phụ trách điều hành các ban chức năng'),
(N'Thành viên', N'Tham gia hoạt động');

INSERT INTO BanChuyenMon (TenBan, MoTa)
VALUES 
(N'Ban Truyền thông', N'Quản lý hình ảnh và truyền thông'),
(N'Ban Sự kiện', N'Tổ chức sự kiện CLB'),
(N'Ban Hậu cần', N'Chuẩn bị vật tư và hậu cần'),
(N'Ban Nghệ Thuật', N'Ngươi mặt đại diện CLB qua các tiết mục văn nghệ và hỗ trợ nhân sự');

INSERT INTO LoaiHoatDong (TenLoaiHD, MoTa)
VALUES 
(N'Tình nguyện', N'Hoạt động vì cộng đồng'),
(N'Học thuật', N'Chia sẻ kiến thức và kỹ năng'),
(N'Giải trí', N'Team building và giao lưu');

INSERT INTO NguonThu (TenNguon, MoTa)
VALUES 
(N'Tài trợ', N'Các doanh nghiệp tài trợ'),
(N'Đóng góp thành viên', N'Các khoản đóng góp định kỳ'),
(N'Hoạt động gây quỹ', N'Thu từ sự kiện CLB');

INSERT INTO KyNang (TenKN, MoTa, CapDo)
VALUES 
(N'Giao tiếp', N'Thuyết trình, trình bày', N'Cơ bản'),
(N'Làm việc nhóm', N'Hợp tác hiệu quả', N'Trung bình'),
(N'Thiết kế', N'Sử dụng Canva, Photoshop', N'Nâng cao');


-- =============================================
-- 2️⃣ THÀNH VIÊN
-- =============================================
INSERT INTO ThanhVien (HoTen, NgaySinh, GioiTinh, Lop, Khoa, SDT, Email, DiaChi, VaiTro, MaCV, MaBan)
VALUES
(N'Nguyễn Vương Khang', '2004-02-15', N'Nam', N'DHKTPM17A', N'Công nghệ thông tin', '0912345678', N'huytm@gmail.com', N'Quận 7, TP.HCM', N'Chủ nhiệm', 1, 1),
(N'Nguyễn Thị Lan', '2005-05-10', N'Nữ', N'DHKTPM17A', N'Công nghệ thông tin', '0987654321', N'lannt@gmail.com', N'Quận 5, TP.HCM', N'Phó chủ nhiệm', 2, 2),
(N'Lê Quốc Bảo', '2005-09-21', N'Nam', N'DHKTPM17B', N'Công nghệ thông tin', '0977112233', N'baolq@gmail.com', N'Quận 10, TP.HCM', N'Thành viên', 4, 3),
(N'Trần Trung Thông', '2005-01-09', N'Nam', N'DHKTPM17B', N'Công nghệ thông tin', '0977112233', N'trantrungthong@gmail.com', N'Quận 10, TP.HCM', N'Thành viên', 4,4 );
-- Cập nhật Trưởng ban
UPDATE BanChuyenMon SET TruongBan = 1 WHERE MaBan = 1;
UPDATE BanChuyenMon SET TruongBan = 2 WHERE MaBan = 2;
UPDATE BanChuyenMon SET TruongBan = 3 WHERE MaBan = 3;
UPDATE BanChuyenMon SET TruongBan = 4 WHERE MaBan = 4;

select * from ThanhVien;
-- =============================================
-- 3️⃣ TÀI KHOẢN
-- =============================================
INSERT INTO TaiKhoan (TenDN, MatKhau, MaTV, QuyenHan)
VALUES 
(N'khangvn', N'123456', 1, N'Quản trị viên'),
(N'lannt', N'123456', 2, N'Thành viên'),
(N'baolq', N'123456', 3, N'Thành viên'),
(N'thong', N'123456', 4, N'Admin');
select MaTV from ThanhVien;

-- =============================================
-- 4️⃣ HOẠT ĐỘNG
-- =============================================
INSERT INTO HoatDong (TenHD, NgayToChuc, DiaDiem, MoTa, MaLoaiHD, NguoiPhuTrach, KinhPhiDuKien, TrangThai)
VALUES 
(N'Chiến dịch Mùa hè xanh', '2025-07-10', N'Củ Chi', N'Tình nguyện giúp đỡ địa phương', 1, 1, 5000000, N'Hoàn thành'),
(N'Hội thảo kỹ năng mềm', '2025-08-15', N'Hội trường B', N'Tăng kỹ năng giao tiếp cho thành viên', 2, 2, 2000000, N'Đã tổ chức'),
(N'Dã ngoại CLB', '2025-09-05', N'Vũng Tàu', N'Team building và nghỉ dưỡng', 3, 3, 3000000, N'Sắp diễn ra');


-- =============================================
-- 5️⃣ DỰ ÁN & PHÂN CÔNG
-- =============================================
INSERT INTO DuAn (TenDuAn, MoTa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES
(N'Hệ thống quản lý CLB', N'Tạo website quản lý thành viên và hoạt động', '2025-05-01', '2025-10-01', N'Đang thực hiện');

INSERT INTO PhanCong (MaTV, MaDA, NhiemVu, TrangThai)
VALUES
(1, 1, N'Thiết kế cơ sở dữ liệu', N'Hoàn thành'),
(2, 1, N'Giao diện web', N'Đang thực hiện'),
(3, 1, N'Kiểm thử chức năng', N'Chưa hoàn thành');


-- =============================================
-- 6️⃣ THU CHI
-- =============================================
INSERT INTO ThuChi (LoaiGD, SoTien, NoiDung, NguoiThucHien, MaNguon)
VALUES 
(N'Thu', 2000000, N'Tài trợ từ công ty ABC', 1, 1),
(N'Chi', 1000000, N'Mua vật tư sự kiện', 2, 3);

INSERT INTO ThuChi_ChiTiet (MaGD, NoiDung, SoTien)
VALUES
(1, N'Tiền mặt nhận tài trợ', 2000000),
(2, N'Mua nước uống và banner', 1000000);


-- =============================================
-- 7️⃣ TÀI SẢN
-- =============================================
INSERT INTO TaiSan (TenTS, SoLuong, DonViTinh, NguoiQuanLy)
VALUES 
(N'Loa di động', 2, N'Bộ', 1),
(N'Bàn gấp', 10, N'Cái', 2),
(N'Máy chiếu', 1, N'Cái', 3);


-- =============================================
-- 8️⃣ LỊCH HỌP & ĐIỂM DANH
-- =============================================
INSERT INTO LichHop (NgayHop, DiaDiem, NoiDung, NguoiChuTri)
VALUES 
('2025-04-15', N'Phòng họp A101', N'Họp tổng kết quý I', 1);

INSERT INTO DiemDanhLichHop (MaLH, MaTV, TrangThai)
VALUES 
(1, 1, N'Có mặt'),
(1, 2, N'Có mặt'),
(1, 3, N'Vắng');


-- =============================================
-- 9️⃣ KHEN THƯỞNG / KỶ LUẬT
-- =============================================
INSERT INTO KhenThuong (MaTV, LyDo)
VALUES 
(1, N'Hoàn thành xuất sắc nhiệm vụ'),
(2, N'Tích cực tham gia hoạt động CLB');

INSERT INTO KyLuat (MaTV, LyDo)
VALUES 
(3, N'Không tham gia họp định kỳ');


-- =============================================
-- 🔟 FEEDBACK / THÔNG BÁO / TIN NHẮN
-- =============================================
INSERT INTO Feedback (MaTV, MaHD, NoiDung)
VALUES 
(2, 1, N'Hoạt động rất ý nghĩa và vui vẻ!'),
(3, 2, N'Nên tổ chức thêm buổi thực hành');

INSERT INTO ThongBao (TieuDe, NoiDung, NguoiDang)
VALUES
(N'Lịch họp tháng 5', N'Mời toàn thể thành viên họp vào 15/5 tại phòng A101', 1),
(N'Đóng góp quỹ CLB', N'Nhắc nhở đóng góp định kỳ 100.000đ', 2);

INSERT INTO TinNhan (MaNguoiGui, MaNguoiNhan, NoiDung)
VALUES
(1, 2, N'Lan ơi, chuẩn bị tài liệu hội thảo nhé!'),
(2, 1, N'Vâng anh Huy, em đã chuẩn bị xong ạ!');


-- =============================================
-- 1️⃣1️⃣ ĐIỂM RÈN LUYỆN
-- =============================================
INSERT INTO DiemRenLuyen (MaTV, HocKy, NamHoc, Diem)
VALUES 
(1, N'HK1', N'2025-2026', 95),
(2, N'HK1', N'2025-2026', 88),
(3, N'HK1', N'2025-2026', 70);


-- =============================================
-- 1️⃣2️⃣ LỊCH SỬ THAO TÁC
-- =============================================
INSERT INTO LichSuThaoTac (MaTV, TenBang, LoaiThaoTac, KhoaChinh, NoiDung)
VALUES
(1, N'HoatDong', N'Thêm', N'MaHD=1', N'Tạo hoạt động Mùa hè xanh'),
(2, N'ThanhVien', N'Cập nhật', N'MaTV=2', N'Cập nhật số điện thoại');
GO

-- Xóa tất cả mật khẩu đã hash sai
UPDATE TaiKhoan SET MatKhau = '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92' WHERE TenDN = 'lannt'
UPDATE TaiKhoan SET MatKhau = '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92' WHERE TenDN = 'baolq'
UPDATE TaiKhoan SET MatKhau = '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92' WHERE TenDN = 'khangvn'
UPDATE TaiKhoan SET MatKhau = '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92' WHERE TenDN = 'thong'
-- Kiểm tra
SELECT * FROM TaiKhoan


