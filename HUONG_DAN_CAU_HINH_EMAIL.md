# Hướng dẫn cấu hình Gmail để gửi email

## Bước 1: Tạo App Password cho Gmail

1. Đăng nhập vào tài khoản Gmail của bạn
2. Truy cập: https://myaccount.google.com/
3. Chọn **Bảo mật** (Security) ở menu bên trái
4. Bật **Xác minh 2 bước** (2-Step Verification) nếu chưa bật
5. Cuộn xuống và tìm **Mật khẩu ứng dụng** (App passwords)
6. Chọn **Ứng dụng** là "Mail" và **Thiết bị** là "Other (Custom name)"
7. Nhập tên: "Vua Sach Cu Website"
8. Click **Tạo** (Generate)
9. **Copy mật khẩu 16 ký tự** được tạo ra (ví dụ: `abcd efgh ijkl mnop`)

## Bước 2: Cấu hình trong appsettings.json

Mở file `appsettings.json` và cập nhật thông tin email:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "Vua Sách Cũ",
    "SenderPassword": "your-16-char-app-password",
    "EnableSsl": true
  }
}
```

**Lưu ý quan trọng:**
- `SenderEmail`: Email Gmail của bạn (ví dụ: `vuasachcuonline@gmail.com`)
- `SenderPassword`: **Mật khẩu ứng dụng 16 ký tự** (không phải mật khẩu Gmail thông thường)
- Không có khoảng trắng trong mật khẩu ứng dụng (ví dụ: `abcdefghijklmnop`)

## Bước 3: Kiểm tra cấu hình

1. Chạy ứng dụng
2. Vào trang **Quên mật khẩu**
3. Nhập email đã đăng ký trong hệ thống
4. Kiểm tra hộp thư email để xem email đặt lại mật khẩu

## Troubleshooting

### Lỗi: "Authentication failed"
- Kiểm tra lại mật khẩu ứng dụng (App Password) đã đúng chưa
- Đảm bảo đã bật **Xác minh 2 bước** trước khi tạo App Password
- Xóa khoảng trắng trong mật khẩu ứng dụng

### Lỗi: "Connection timeout"
- Kiểm tra kết nối internet
- Kiểm tra firewall có chặn port 587 không
- Thử đổi `SmtpPort` thành `465` và `EnableSsl` thành `true`

### Email không đến
- Kiểm tra thư mục **Spam/Junk**
- Kiểm tra email đã được nhập đúng trong hệ thống chưa
- Xem log trong console để biết lỗi chi tiết

## Lưu ý bảo mật

⚠️ **KHÔNG** commit file `appsettings.json` có chứa mật khẩu thật lên Git!

Nên sử dụng:
- `appsettings.Development.json` cho môi trường development (có thể commit)
- `appsettings.Production.json` cho môi trường production (KHÔNG commit, chỉ cấu hình trên server)

Hoặc sử dụng **User Secrets** hoặc **Environment Variables** để lưu mật khẩu.

## Cấu hình User Secrets (Khuyến nghị)

Chạy lệnh trong terminal:

```bash
dotnet user-secrets init
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:SenderPassword" "your-app-password"
```

Sau đó xóa thông tin nhạy cảm khỏi `appsettings.json`.

