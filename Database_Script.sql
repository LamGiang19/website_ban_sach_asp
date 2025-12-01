
-- Tạo Database (nếu chưa có)
-- CREATE DATABASE WebBanSachDB;
-- GO
-- USE WebBanSachDB;
-- GO

-- =============================================
-- 1. Bảng DanhMuc (Categories)
-- =============================================
CREATE TABLE DanhMuc (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(255) NOT NULL,
    MoTa NVARCHAR(MAX) NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 1
);
GO

-- =============================================
-- 2. Bảng TacGia (Authors)
-- =============================================
CREATE TABLE TacGia (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenTacGia NVARCHAR(255) NOT NULL,
    GioiThieu NVARCHAR(MAX) NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 1
);
GO

-- =============================================
-- 3. Bảng NhaXuatBan (Publishers)
-- =============================================
CREATE TABLE NhaXuatBan (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenNhaXuatBan NVARCHAR(255) NOT NULL,
    DiaChi NVARCHAR(500) NULL,
    SoDienThoai NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 1
);
GO

-- =============================================
-- 4. Bảng TaiKhoan (Accounts)
-- =============================================
CREATE TABLE TaiKhoan (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(100) NOT NULL UNIQUE,
    MatKhau NVARCHAR(255) NOT NULL,
    HoTen NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(20) NULL,
    DiaChi NVARCHAR(500) NULL,
    VaiTro NVARCHAR(50) DEFAULT 'User',
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 1
);
GO

-- =============================================
-- 5. Bảng Sach (Books)
-- =============================================
CREATE TABLE Sach (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenSach NVARCHAR(255) NOT NULL,
    MoTa NVARCHAR(MAX) NULL,
    Gia DECIMAL(18,2) NOT NULL,
    SoLuong INT NOT NULL DEFAULT 0,
    HinhAnh NVARCHAR(500) NULL,
    DanhMucId INT NOT NULL,
    TacGiaId INT NOT NULL,
    NhaXuatBanId INT NOT NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 1,
    CONSTRAINT FK_Sach_DanhMuc FOREIGN KEY (DanhMucId) REFERENCES DanhMuc(Id),
    CONSTRAINT FK_Sach_TacGia FOREIGN KEY (TacGiaId) REFERENCES TacGia(Id),
    CONSTRAINT FK_Sach_NhaXuatBan FOREIGN KEY (NhaXuatBanId) REFERENCES NhaXuatBan(Id)
);
GO

-- =============================================
-- 6. Bảng GioHang (Carts)
-- =============================================
CREATE TABLE GioHang (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TaiKhoanId INT NOT NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_GioHang_TaiKhoan FOREIGN KEY (TaiKhoanId) REFERENCES TaiKhoan(Id)
);
GO

-- =============================================
-- 7. Bảng ChiTietGioHang (CartItems)
-- =============================================
CREATE TABLE ChiTietGioHang (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GioHangId INT NOT NULL,
    SachId INT NOT NULL,
    SoLuong INT NOT NULL DEFAULT 1,
    Gia DECIMAL(18,2) NOT NULL,
    NgayThem DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_ChiTietGioHang_GioHang FOREIGN KEY (GioHangId) REFERENCES GioHang(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietGioHang_Sach FOREIGN KEY (SachId) REFERENCES Sach(Id),
    CONSTRAINT CK_ChiTietGioHang_SoLuong CHECK (SoLuong > 0)
);
GO

-- =============================================
-- 8. Bảng DonHang (Orders)
-- =============================================
CREATE TABLE DonHang (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TaiKhoanId INT NOT NULL,
    NgayDat DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Chờ xử lý',
    DiaChiGiaoHang NVARCHAR(500) NOT NULL,
    SoDienThoai NVARCHAR(20) NOT NULL,
    HoTenNguoiNhan NVARCHAR(255) NOT NULL,
    GhiChu NVARCHAR(MAX) NULL,
    CONSTRAINT FK_DonHang_TaiKhoan FOREIGN KEY (TaiKhoanId) REFERENCES TaiKhoan(Id),
    CONSTRAINT CK_DonHang_TongTien CHECK (TongTien >= 0)
);
GO

-- =============================================
-- 9. Bảng ChiTietDonHang (OrderItems)
-- =============================================
CREATE TABLE ChiTietDonHang (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DonHangId INT NOT NULL,
    SachId INT NOT NULL,
    SoLuong INT NOT NULL,
    Gia DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_ChiTietDonHang_DonHang FOREIGN KEY (DonHangId) REFERENCES DonHang(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietDonHang_Sach FOREIGN KEY (SachId) REFERENCES Sach(Id),
    CONSTRAINT CK_ChiTietDonHang_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_ChiTietDonHang_Gia CHECK (Gia >= 0)
);
GO

-- =============================================
-- Tạo Indexes để tối ưu hiệu suất
-- =============================================

-- Index cho bảng Sach
CREATE INDEX IX_Sach_DanhMucId ON Sach(DanhMucId);
CREATE INDEX IX_Sach_TacGiaId ON Sach(TacGiaId);
CREATE INDEX IX_Sach_NhaXuatBanId ON Sach(NhaXuatBanId);
CREATE INDEX IX_Sach_TenSach ON Sach(TenSach);
GO

-- Index cho bảng TaiKhoan
CREATE INDEX IX_TaiKhoan_TenDangNhap ON TaiKhoan(TenDangNhap);
CREATE INDEX IX_TaiKhoan_Email ON TaiKhoan(Email);
GO

-- Index cho bảng GioHang
CREATE INDEX IX_GioHang_TaiKhoanId ON GioHang(TaiKhoanId);
GO

-- Index cho bảng ChiTietGioHang
CREATE INDEX IX_ChiTietGioHang_GioHangId ON ChiTietGioHang(GioHangId);
CREATE INDEX IX_ChiTietGioHang_SachId ON ChiTietGioHang(SachId);
GO

-- Index cho bảng DonHang
CREATE INDEX IX_DonHang_TaiKhoanId ON DonHang(TaiKhoanId);
CREATE INDEX IX_DonHang_NgayDat ON DonHang(NgayDat);
CREATE INDEX IX_DonHang_TrangThai ON DonHang(TrangThai);
GO

-- Index cho bảng ChiTietDonHang
CREATE INDEX IX_ChiTietDonHang_DonHangId ON ChiTietDonHang(DonHangId);
CREATE INDEX IX_ChiTietDonHang_SachId ON ChiTietDonHang(SachId);
GO

-- =============================================
-- INSERT DỮ LIỆU MẪU
-- Lưu ý: Phải insert theo thứ tự để tránh lỗi Foreign Key
-- Thứ tự: DanhMuc -> TacGia -> NhaXuatBan -> TaiKhoan -> Sach -> GioHang -> ChiTietGioHang -> DonHang -> ChiTietDonHang
-- =============================================

-- BƯỚC 1: Insert DanhMuc (Không có Foreign Key)
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES
(N'Tiểu thuyết', N'Các tác phẩm tiểu thuyết văn học Việt Nam và thế giới'),
(N'Khoa học', N'Sách về khoa học, công nghệ và nghiên cứu'),
(N'Lịch sử', N'Sách về lịch sử Việt Nam và thế giới'),
(N'Kinh tế', N'Sách về kinh tế, tài chính và đầu tư'),
(N'Thiếu nhi', N'Sách dành cho trẻ em và thanh thiếu niên'),
(N'Self-help', N'Sách phát triển bản thân và kỹ năng sống'),
(N'Văn học cổ điển', N'Các tác phẩm văn học kinh điển'),
(N'Kỹ năng', N'Sách hướng dẫn kỹ năng và nghề nghiệp'),
(N'Tâm lý học', N'Sách về tâm lý học và hành vi con người'),
(N'Du lịch', N'Sách về du lịch và khám phá');
GO

-- BƯỚC 2: Insert TacGia (Không có Foreign Key)
INSERT INTO TacGia (TenTacGia, GioiThieu) VALUES
(N'Nguyễn Du', N'Đại thi hào dân tộc Việt Nam, tác giả Truyện Kiều'),
(N'Nam Cao', N'Nhà văn hiện thực xuất sắc của văn học Việt Nam'),
(N'Ngô Tất Tố', N'Nhà văn, nhà báo nổi tiếng thời kỳ đầu thế kỷ 20'),
(N'Stephen Hawking', N'Nhà vật lý lý thuyết, vũ trụ học nổi tiếng thế giới'),
(N'Dale Carnegie', N'Tác giả sách self-help nổi tiếng, người sáng lập Dale Carnegie Training'),
(N'Nguyễn Nhật Ánh', N'Nhà văn Việt Nam chuyên viết cho thanh thiếu niên'),
(N'Haruki Murakami', N'Nhà văn Nhật Bản nổi tiếng với phong cách hiện thực huyền ảo'),
(N'Paulo Coelho', N'Nhà văn Brazil nổi tiếng với tác phẩm Nhà giả kim'),
(N'Nguyễn Ngọc Tư', N'Nhà văn nữ Việt Nam đương đại'),
(N'Trần Đăng Khoa', N'Nhà thơ, nhà văn Việt Nam'),
(N'J.K. Rowling', N'Nhà văn Anh, tác giả bộ truyện Harry Potter'),
(N'Dan Brown', N'Nhà văn Mỹ chuyên viết tiểu thuyết trinh thám'),
(N'Malcolm Gladwell', N'Nhà báo và tác giả sách về xã hội học'),
(N'Yuval Noah Harari', N'Nhà sử học Israel, tác giả Sapiens'),
(N'Nguyễn Phong Việt', N'Nhà thơ, tác giả trẻ Việt Nam');
GO

-- BƯỚC 3: Insert NhaXuatBan (Không có Foreign Key)
INSERT INTO NhaXuatBan (TenNhaXuatBan, DiaChi, SoDienThoai, Email) VALUES
(N'NXB Kim Đồng', N'55 Quang Trung, Hoàn Kiếm, Hà Nội', '02438253751', 'info@kimdong.vn'),
(N'NXB Trẻ', N'161B Lý Chính Thắng, Phường 7, Quận 3, TP.HCM', '02838423016', 'nxbtre@nxbtre.com.vn'),
(N'NXB Giáo Dục', N'81 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội', '02438220801', 'contact@nxbgd.vn'),
(N'NXB Văn Học', N'18 Nguyễn Trường Tộ, Ba Đình, Hà Nội', '02437161518', 'info@nxbvanhoc.com.vn'),
(N'NXB Hội Nhà Văn', N'65 Nguyễn Du, Hai Bà Trưng, Hà Nội', '02438221301', 'nxbhnv@hnv.vn'),
(N'NXB Phụ Nữ', N'39 Hàng Chuối, Hai Bà Trưng, Hà Nội', '02438221301', 'nxbphunu@nxbphunu.com.vn'),
(N'NXB Thế Giới', N'46 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội', '02438253535', 'info@nxbthegioi.com.vn'),
(N'NXB Tổng Hợp TP.HCM', N'62 Nguyễn Thị Minh Khai, Quận 1, TP.HCM', '02838225741', 'nxbth@nxbth.com.vn');
GO

-- BƯỚC 4: Insert TaiKhoan (Không có Foreign Key)
-- Lưu ý: Mật khẩu cần được hash trong code (ví dụ: admin123, user123)
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, HoTen, Email, SoDienThoai, DiaChi, VaiTro) VALUES
(N'admin', N'$2a$11$...', N'Quản trị viên', N'admin@example.com', '0901234567', N'123 Đường ABC, Quận 1, TP.HCM', N'Admin'),
(N'nguyenvana', N'$2a$11$...', N'Nguyễn Văn A', N'nguyenvana@gmail.com', '0912345678', N'456 Đường XYZ, Quận 2, TP.HCM', N'User'),
(N'tranthib', N'$2a$11$...', N'Trần Thị B', N'tranthib@gmail.com', '0923456789', N'789 Đường DEF, Quận 3, TP.HCM', N'User'),
(N'lethic', N'$2a$11$...', N'Lê Thị C', N'lethic@gmail.com', '0934567890', N'321 Đường GHI, Quận 4, TP.HCM', N'User'),
(N'phamvand', N'$2a$11$...', N'Phạm Văn D', N'phamvand@gmail.com', '0945678901', N'654 Đường JKL, Quận 5, TP.HCM', N'User'),
(N'hoangthie', N'$2a$11$...', N'Hoàng Thị E', N'hoangthie@gmail.com', '0956789012', N'987 Đường MNO, Quận 6, TP.HCM', N'User');
GO

-- BƯỚC 5: Insert Sach (Cần DanhMuc, TacGia, NhaXuatBan đã có)
-- Lưu ý: DanhMucId, TacGiaId, NhaXuatBanId phải tham chiếu đến các bảng đã insert ở trên
INSERT INTO Sach (TenSach, MoTa, Gia, SoLuong, HinhAnh, DanhMucId, TacGiaId, NhaXuatBanId) VALUES
-- Tiểu thuyết
(N'Truyện Kiều', N'Tác phẩm kinh điển của Nguyễn Du, một trong những kiệt tác văn học Việt Nam', 150000, 50, N'/images/sach/truyen-kieu.jpg', 1, 1, 4),
(N'Chí Phèo', N'Tác phẩm nổi tiếng của Nam Cao, phản ánh hiện thực xã hội nông thôn Việt Nam', 80000, 30, N'/images/sach/chi-pheo.jpg', 1, 2, 4),
(N'Lão Hạc', N'Truyện ngắn hay của Nam Cao về số phận người nông dân', 70000, 25, N'/images/sach/lao-hac.jpg', 1, 2, 4),
(N'Tắt Đèn', N'Tác phẩm nổi tiếng của Ngô Tất Tố về cuộc sống người nông dân', 90000, 35, N'/images/sach/tat-den.jpg', 1, 3, 4),
(N'Rừng Xà Nu', N'Truyện ngắn nổi tiếng của Nguyễn Trung Thành', 75000, 28, N'/images/sach/rung-xa-nu.jpg', 1, 10, 4),
(N'Dế Mèn Phiêu Lưu Ký', N'Tác phẩm thiếu nhi kinh điển của Tô Hoài', 85000, 40, N'/images/sach/de-men.jpg', 1, 10, 1),

-- Văn học nước ngoài
(N'Nhà Giả Kim', N'Cuốn sách bestseller của Paulo Coelho về hành trình tìm kiếm ý nghĩa cuộc sống', 120000, 45, N'/images/sach/nha-gia-kim.jpg', 7, 8, 2),
(N'Rừng Na Uy', N'Tiểu thuyết nổi tiếng của Haruki Murakami', 180000, 30, N'/images/sach/rung-na-uy.jpg', 1, 7, 2),
(N'Mật Mã Da Vinci', N'Tiểu thuyết trinh thám hấp dẫn của Dan Brown', 200000, 25, N'/images/sach/mat-ma-da-vinci.jpg', 1, 12, 2),
(N'Harry Potter và Hòn Đá Phù Thủy', N'Cuốn đầu tiên trong bộ truyện Harry Potter', 250000, 60, N'/images/sach/harry-potter-1.jpg', 5, 11, 2),

-- Khoa học
(N'Lược Sử Thời Gian', N'Cuốn sách khoa học nổi tiếng của Stephen Hawking về vũ trụ', 200000, 40, N'/images/sach/luoc-su-thoi-gian.jpg', 2, 4, 2),
(N'Sapiens: Lược Sử Loài Người', N'Cuốn sách bestseller của Yuval Noah Harari về lịch sử loài người', 280000, 35, N'/images/sach/sapiens.jpg', 2, 14, 2),
(N'Vũ Trụ Trong Vỏ Hạt Dẻ', N'Tác phẩm khoa học của Stephen Hawking', 220000, 30, N'/images/sach/vu-tru-trong-vo-hat-de.jpg', 2, 4, 2),

-- Self-help
(N'Đắc Nhân Tâm', N'Sách self-help kinh điển của Dale Carnegie về nghệ thuật giao tiếp', 120000, 60, N'/images/sach/dac-nhan-tam.jpg', 6, 5, 2),
(N'Người Bán Hàng Vĩ Đại Nhất Thế Giới', N'Cuốn sách về bí quyết thành công', 100000, 50, N'/images/sach/nguoi-ban-hang-vi-dai.jpg', 6, 5, 2),
(N'7 Thói Quen Của Người Thành Đạt', N'Sách về phát triển bản thân của Stephen Covey', 150000, 45, N'/images/sach/7-thoi-quen.jpg', 6, 5, 2),
(N'Think and Grow Rich', N'Cuốn sách kinh điển về thành công và giàu có', 130000, 40, N'/images/sach/think-and-grow-rich.jpg', 6, 5, 2),

-- Kinh tế
(N'Kinh Tế Học Vui Vẻ', N'Cuốn sách giải thích kinh tế học một cách dễ hiểu', 160000, 35, N'/images/sach/kinh-te-hoc-vui-ve.jpg', 4, 13, 2),
(N'Rich Dad Poor Dad', N'Cuốn sách về tài chính cá nhân nổi tiếng', 140000, 50, N'/images/sach/rich-dad-poor-dad.jpg', 4, 5, 2),
(N'Bí Mật Tư Duy Triệu Phú', N'Sách về tư duy và thái độ của người giàu', 110000, 40, N'/images/sach/bi-mat-tu-duy-trieu-phu.jpg', 4, 5, 2),

-- Thiếu nhi
(N'Kính Vạn Hoa', N'Bộ truyện thiếu nhi nổi tiếng của Nguyễn Nhật Ánh', 95000, 55, N'/images/sach/kinh-van-hoa.jpg', 5, 6, 1),
(N'Tôi Thấy Hoa Vàng Trên Cỏ Xanh', N'Tác phẩm thiếu nhi hay của Nguyễn Nhật Ánh', 110000, 50, N'/images/sach/toi-thay-hoa-vang.jpg', 5, 6, 1),
(N'Cho Tôi Xin Một Vé Đi Tuổi Thơ', N'Cuốn sách thiếu nhi đầy cảm xúc của Nguyễn Nhật Ánh', 105000, 48, N'/images/sach/cho-toi-xin-mot-ve.jpg', 5, 6, 1),
(N'Cô Gái Đến Từ Hôm Qua', N'Tác phẩm thiếu nhi của Nguyễn Nhật Ánh', 100000, 45, N'/images/sach/co-gai-den-tu-hom-qua.jpg', 5, 6, 1),

-- Kỹ năng
(N'Kỹ Năng Giao Tiếp Hiệu Quả', N'Sách về kỹ năng giao tiếp và ứng xử', 90000, 40, N'/images/sach/ky-nang-giao-tiep.jpg', 8, 5, 2),
(N'Làm Chủ Tư Duy Thay Đổi Vận Mệnh', N'Cuốn sách về tư duy và thay đổi bản thân', 125000, 35, N'/images/sach/lam-chu-tu-duy.jpg', 8, 5, 2),

-- Tâm lý học
(N'Tâm Lý Học Đám Đông', N'Cuốn sách kinh điển về tâm lý học xã hội', 140000, 30, N'/images/sach/tam-ly-hoc-dam-dong.jpg', 9, 13, 2),
(N'Điểm Bùng Phát', N'Cuốn sách về cách ý tưởng lan truyền của Malcolm Gladwell', 160000, 32, N'/images/sach/diem-bung-phat.jpg', 9, 13, 2),

-- Lịch sử
(N'Đại Việt Sử Ký Toàn Thư', N'Bộ sử ký quan trọng của Việt Nam', 300000, 20, N'/images/sach/dai-viet-su-ky.jpg', 3, 10, 3),
(N'Lịch Sử Việt Nam', N'Cuốn sách tổng hợp về lịch sử Việt Nam', 180000, 25, N'/images/sach/lich-su-viet-nam.jpg', 3, 10, 3);
GO

-- BƯỚC 6: Insert GioHang (Cần TaiKhoan đã có)
-- Lưu ý: TaiKhoanId phải tham chiếu đến các tài khoản đã insert (Id từ 1-6)
INSERT INTO GioHang (TaiKhoanId, NgayTao) VALUES
(2, DATEADD(day, -2, GETDATE())),  -- Giỏ hàng của nguyenvana (TaiKhoanId = 2)
(3, DATEADD(day, -1, GETDATE())),  -- Giỏ hàng của tranthib (TaiKhoanId = 3)
(4, GETDATE()),                     -- Giỏ hàng của lethic (TaiKhoanId = 4)
(5, DATEADD(day, -3, GETDATE()));  -- Giỏ hàng của phamvand (TaiKhoanId = 5)
GO

-- BƯỚC 7: Insert ChiTietGioHang (Cần GioHang và Sach đã có)
-- Lưu ý: GioHangId tham chiếu đến GioHang (Id từ 1-4), SachId tham chiếu đến Sach (Id từ 1-30)
INSERT INTO ChiTietGioHang (GioHangId, SachId, SoLuong, Gia) VALUES
-- Giỏ hàng của nguyenvana (Id=2)
(1, 1, 2, 150000),  -- Truyện Kiều x2
(1, 5, 1, 75000),   -- Rừng Xà Nu x1
-- Giỏ hàng của tranthib (Id=3)
(2, 7, 1, 120000),  -- Nhà Giả Kim x1
(2, 14, 2, 120000), -- Đắc Nhân Tâm x2
(2, 20, 1, 110000), -- Tôi Thấy Hoa Vàng x1
-- Giỏ hàng của lethic (Id=4)
(3, 11, 1, 200000), -- Lược Sử Thời Gian x1
(3, 12, 1, 280000), -- Sapiens x1
-- Giỏ hàng của phamvand (Id=5)
(4, 8, 1, 180000),  -- Rừng Na Uy x1
(4, 9, 1, 200000);  -- Mật Mã Da Vinci x1
GO

-- BƯỚC 8: Insert DonHang (Cần TaiKhoan đã có)
-- Lưu ý: TaiKhoanId phải tham chiếu đến các tài khoản đã insert (Id từ 1-6)
INSERT INTO DonHang (TaiKhoanId, NgayDat, TongTien, TrangThai, DiaChiGiaoHang, SoDienThoai, HoTenNguoiNhan, GhiChu) VALUES
(2, DATEADD(day, -10, GETDATE()), 330000, N'Đã giao', N'456 Đường XYZ, Quận 2, TP.HCM', '0912345678', N'Nguyễn Văn A', N'Giao trong giờ hành chính'),  -- Đơn hàng của nguyenvana (70000+90000+170000)
(3, DATEADD(day, -7, GETDATE()), 350000, N'Đang giao', N'789 Đường DEF, Quận 3, TP.HCM', '0923456789', N'Trần Thị B', N'Giao trước 17h'),              -- Đơn hàng của tranthib
(4, DATEADD(day, -5, GETDATE()), 700000, N'Chờ xử lý', N'321 Đường GHI, Quận 4, TP.HCM', '0934567890', N'Lê Thị C', NULL),                              -- Đơn hàng của lethic (sửa tổng tiền)
(5, DATEADD(day, -3, GETDATE()), 380000, N'Đã giao', N'654 Đường JKL, Quận 5, TP.HCM', '0945678901', N'Phạm Văn D', N'Cảm ơn shop'),                    -- Đơn hàng của phamvand
(2, DATEADD(day, -1, GETDATE()), 250000, N'Chờ xử lý', N'456 Đường XYZ, Quận 2, TP.HCM', '0912345678', N'Nguyễn Văn A', NULL);                          -- Đơn hàng thứ 2 của nguyenvana
GO

-- BƯỚC 9: Insert ChiTietDonHang (Cần DonHang và Sach đã có)
-- Lưu ý: DonHangId tham chiếu đến DonHang (Id từ 1-5), SachId tham chiếu đến Sach (Id từ 1-30)
INSERT INTO ChiTietDonHang (DonHangId, SachId, SoLuong, Gia) VALUES
-- Đơn hàng 1 của nguyenvana
(1, 3, 1, 70000),   -- Lão Hạc
(1, 4, 1, 90000),   -- Tắt Đèn
(1, 6, 2, 85000),   -- Dế Mèn Phiêu Lưu Ký x2
-- Đơn hàng 2 của tranthib
(2, 15, 1, 120000), -- Đắc Nhân Tâm
(2, 16, 1, 100000), -- Người Bán Hàng Vĩ Đại
(2, 18, 1, 130000), -- Think and Grow Rich
-- Đơn hàng 3 của lethic
(3, 11, 1, 200000), -- Lược Sử Thời Gian
(3, 13, 1, 220000), -- Vũ Trụ Trong Vỏ Hạt Dẻ
(3, 12, 1, 280000), -- Sapiens
-- Đơn hàng 4 của phamvand
(4, 8, 1, 180000),  -- Rừng Na Uy
(4, 9, 1, 200000),  -- Mật Mã Da Vinci
-- Đơn hàng 5 của nguyenvana
(5, 10, 1, 250000); -- Harry Potter
GO

-

