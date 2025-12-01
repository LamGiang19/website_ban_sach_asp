# DANH SÁCH CHỨC NĂNG VÀ FILE CẦN THIẾT CHO WEBSITE BÁN SÁCH

## 📋 TỔNG QUAN CHỨC NĂNG

### 1. QUẢN LÝ SẢN PHẨM (SÁCH)
- Xem danh sách sách
- Xem chi tiết sách
- Tìm kiếm sách
- Lọc sách theo danh mục
- Lọc sách theo tác giả
- Lọc sách theo giá
- Sắp xếp sách (giá, tên, mới nhất)
- Phân trang danh sách sách

### 2. QUẢN LÝ DANH MỤC
- Xem danh sách danh mục
- Xem sách theo danh mục

### 3. QUẢN LÝ GIỎ HÀNG
- Thêm sách vào giỏ hàng
- Xem giỏ hàng
- Cập nhật số lượng sách trong giỏ hàng
- Xóa sách khỏi giỏ hàng
- Tính tổng tiền giỏ hàng

### 4. QUẢN LÝ ĐƠN HÀNG
- Đặt hàng
- Xem lịch sử đơn hàng
- Xem chi tiết đơn hàng
- Hủy đơn hàng (nếu chưa xử lý)

### 5. QUẢN LÝ TÀI KHOẢN
- Đăng ký tài khoản
- Đăng nhập
- Đăng xuất
- Xem thông tin tài khoản
- Cập nhật thông tin cá nhân
- Đổi mật khẩu

### 6. QUẢN TRỊ (ADMIN) - Tùy chọn
- Quản lý sách (CRUD)
- Quản lý danh mục (CRUD)
- Quản lý đơn hàng
- Quản lý người dùng
- Thống kê doanh thu

---

## 📁 CẤU TRÚC FILE CẦN TẠO

### 📂 MODELS (Models/)

#### Models/Entities/
```
📄 Sach.cs                    - Model sách
📄 DanhMuc.cs                 - Model danh mục
📄 TacGia.cs                  - Model tác giả
📄 NhaXuatBan.cs              - Model nhà xuất bản
📄 GioHang.cs                 - Model giỏ hàng
📄 ChiTietGioHang.cs          - Model chi tiết giỏ hàng
📄 DonHang.cs                 - Model đơn hàng
📄 ChiTietDonHang.cs          - Model chi tiết đơn hàng
📄 KhachHang.cs               - Model khách hàng
📄 TaiKhoan.cs                - Model tài khoản (nếu dùng Identity)
```

#### Models/ViewModels/
```
📄 SachViewModel.cs           - ViewModel cho sách
📄 GioHangViewModel.cs        - ViewModel cho giỏ hàng
📄 DonHangViewModel.cs        - ViewModel cho đơn hàng
📄 DangKyViewModel.cs         - ViewModel đăng ký
📄 DangNhapViewModel.cs       - ViewModel đăng nhập
📄 TimKiemViewModel.cs        - ViewModel tìm kiếm
```

---

### 📂 CONTROLLERS (Controllers/)

```
📄 HomeController.cs          - Trang chủ (đã có, cần cập nhật)
📄 SachController.cs          - Quản lý sách (xem, tìm kiếm, lọc)
📄 GioHangController.cs       - Quản lý giỏ hàng
📄 DonHangController.cs       - Quản lý đơn hàng
📄 TaiKhoanController.cs     - Quản lý tài khoản (đăng ký, đăng nhập)
📄 DanhMucController.cs       - Quản lý danh mục
📄 AdminController.cs         - Quản trị (tùy chọn)
```

---

### 📂 VIEWS (Views/)

#### Views/Home/
```
📄 Index.cshtml               - Trang chủ (hiển thị sách nổi bật)
📄 Privacy.cshtml             - Đã có
```

#### Views/Sach/
```
📄 Index.cshtml               - Danh sách sách (có phân trang, lọc, sắp xếp)
📄 Details.cshtml             - Chi tiết sách
📄 Search.cshtml              - Kết quả tìm kiếm
```

#### Views/GioHang/
```
📄 Index.cshtml               - Xem giỏ hàng
```

#### Views/DonHang/
```
📄 Create.cshtml              - Form đặt hàng
📄 Index.cshtml               - Lịch sử đơn hàng
📄 Details.cshtml             - Chi tiết đơn hàng
```

#### Views/TaiKhoan/
```
📄 DangKy.cshtml              - Form đăng ký
📄 DangNhap.cshtml            - Form đăng nhập
📄 ThongTin.cshtml            - Thông tin tài khoản
📄 DoiMatKhau.cshtml          - Đổi mật khẩu
```

#### Views/DanhMuc/
```
📄 Index.cshtml               - Danh sách danh mục
📄 Details.cshtml             - Sách theo danh mục
```

#### Views/Shared/
```
📄 _Layout.cshtml             - Layout chính (đã có, cần cập nhật)
📄 _LayoutAdmin.cshtml        - Layout admin (nếu có)
📄 _Header.cshtml             - Header với menu
📄 _Footer.cshtml             - Footer
📄 _GioHangPartial.cshtml     - Partial view giỏ hàng (hiển thị số lượng)
📄 _DanhMucPartial.cshtml     - Partial view danh mục (sidebar)
📄 _SachCard.cshtml           - Partial view card sách (tái sử dụng)
```

---

### 📂 DATA (Data/)

```
📄 ApplicationDbContext.cs    - DbContext chính
📄 DbInitializer.cs           - Khởi tạo dữ liệu mẫu (tùy chọn)
```

---

### 📂 SERVICES (Services/) - Tùy chọn

```
📄 ISachService.cs            - Interface service sách
📄 SachService.cs             - Service sách
📄 IGioHangService.cs         - Interface service giỏ hàng
📄 GioHangService.cs          - Service giỏ hàng
📄 IDonHangService.cs         - Interface service đơn hàng
📄 DonHangService.cs          - Service đơn hàng
```

---

### 📂 REPOSITORIES (Repositories/) - Tùy chọn

```
📄 IRepository.cs             - Interface repository generic
📄 Repository.cs              - Repository generic
📄 ISachRepository.cs         - Interface repository sách
📄 SachRepository.cs          - Repository sách
```

---

### 📂 HELPERS (Helpers/)

```
📄 SessionHelper.cs           - Helper quản lý session
📄 ImageHelper.cs             - Helper xử lý hình ảnh
📄 FormatHelper.cs            - Helper format tiền, ngày tháng
```

---

### 📂 WWWROOT (wwwroot/)

#### wwwroot/images/
```
📁 sach/                      - Thư mục chứa hình ảnh sách
```

#### wwwroot/css/
```
📄 site.css                   - CSS chính (đã có, cần cập nhật)
📄 giohang.css                - CSS cho giỏ hàng
📄 sach.css                   - CSS cho trang sách
```

#### wwwroot/js/
```
📄 site.js                    - JS chính (đã có, cần cập nhật)
📄 giohang.js                 - JS cho giỏ hàng (AJAX)
📄 timkiem.js                 - JS cho tìm kiếm
```

---

### 📂 CONFIGURATION

```
📄 appsettings.json           - Cấu hình (đã có, cần thêm connection string)
📄 Program.cs                 - Cấu hình services (đã có, cần cập nhật)
```

---

## 🗄️ DATABASE SCHEMA

### Bảng cần tạo:

1. **DanhMuc** (Categories)
   - Id (int, PK)
   - TenDanhMuc (nvarchar)
   - MoTa (nvarchar, nullable)

2. **TacGia** (Authors)
   - Id (int, PK)
   - TenTacGia (nvarchar)
   - GioiThieu (nvarchar, nullable)

3. **NhaXuatBan** (Publishers)
   - Id (int, PK)
   - TenNhaXuatBan (nvarchar)
   - DiaChi (nvarchar, nullable)

4. **Sach** (Books)
   - Id (int, PK)
   - TenSach (nvarchar)
   - MoTa (nvarchar, nullable)
   - Gia (decimal)
   - SoLuong (int)
   - HinhAnh (nvarchar, nullable)
   - DanhMucId (int, FK)
   - TacGiaId (int, FK)
   - NhaXuatBanId (int, FK)
   - NgayTao (datetime)
   - TrangThai (bit)

5. **TaiKhoan** (Accounts) - Hoặc dùng Identity
   - Id (int, PK)
   - TenDangNhap (nvarchar, unique)
   - MatKhau (nvarchar) - Hash
   - HoTen (nvarchar)
   - Email (nvarchar)
   - SoDienThoai (nvarchar, nullable)
   - DiaChi (nvarchar, nullable)
   - VaiTro (nvarchar) - "User" hoặc "Admin"

6. **GioHang** (Carts)
   - Id (int, PK)
   - TaiKhoanId (int, FK)
   - NgayTao (datetime)

7. **ChiTietGioHang** (CartItems)
   - Id (int, PK)
   - GioHangId (int, FK)
   - SachId (int, FK)
   - SoLuong (int)
   - Gia (decimal)

8. **DonHang** (Orders)
   - Id (int, PK)
   - TaiKhoanId (int, FK)
   - NgayDat (datetime)
   - TongTien (decimal)
   - TrangThai (nvarchar) - "Chờ xử lý", "Đang giao", "Đã giao", "Đã hủy"
   - DiaChiGiaoHang (nvarchar)
   - SoDienThoai (nvarchar)
   - GhiChu (nvarchar, nullable)

9. **ChiTietDonHang** (OrderItems)
   - Id (int, PK)
   - DonHangId (int, FK)
   - SachId (int, FK)
   - SoLuong (int)
   - Gia (decimal)

---

## 📦 NUGET PACKAGES CẦN CÀI ĐẶT

```
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer (hoặc SQLite cho dev)
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design
Microsoft.AspNetCore.Identity.EntityFrameworkCore (nếu dùng Identity)
```

---

## 🔧 CÁC BƯỚC TRIỂN KHAI

### Bước 1: Setup Database
- [ ] Tạo DbContext
- [ ] Tạo các Model entities
- [ ] Tạo Migration
- [ ] Update Database
- [ ] Seed dữ liệu mẫu

### Bước 2: Models & ViewModels
- [ ] Tạo tất cả Models
- [ ] Tạo ViewModels

### Bước 3: Controllers & Views
- [ ] HomeController (trang chủ)
- [ ] SachController (danh sách, chi tiết, tìm kiếm)
- [ ] DanhMucController
- [ ] GioHangController
- [ ] DonHangController
- [ ] TaiKhoanController

### Bước 4: Views
- [ ] Layout chính với header/footer
- [ ] Trang chủ
- [ ] Danh sách sách
- [ ] Chi tiết sách
- [ ] Giỏ hàng
- [ ] Đặt hàng
- [ ] Đăng ký/Đăng nhập

### Bước 5: Chức năng nâng cao
- [ ] Session/Cookie cho giỏ hàng
- [ ] Tìm kiếm nâng cao
- [ ] Phân trang
- [ ] Upload hình ảnh

### Bước 6: Admin (Tùy chọn)
- [ ] Admin Controller
- [ ] CRUD sách
- [ ] CRUD danh mục
- [ ] Quản lý đơn hàng

---

## 📝 GHI CHÚ

- Có thể sử dụng **Entity Framework Core** cho ORM
- Có thể sử dụng **ASP.NET Core Identity** cho authentication
- Có thể sử dụng **Session** hoặc **Cookie** để lưu giỏ hàng (nếu chưa đăng nhập)
- Nên tách logic business ra **Services** layer
- Nên sử dụng **Repository Pattern** nếu dự án lớn
- Có thể thêm **AutoMapper** để map Entity -> ViewModel

---

## 🎨 UI/UX GỢI Ý

- Responsive design (mobile-friendly)
- Hiển thị sách dạng grid/card
- Có breadcrumb navigation
- Có pagination cho danh sách
- Có loading indicator
- Validation form rõ ràng
- Thông báo thành công/lỗi

