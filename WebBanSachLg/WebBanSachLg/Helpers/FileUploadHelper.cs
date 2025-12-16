namespace WebBanSachLg.Helpers
{
    public static class FileUploadHelper
    {
        private const string SACH_IMAGE_FOLDER = "images/sach";

        public static async Task<string?> UploadImageAsync(IFormFile file, string webRootPath, string folderPath)
        {
            if (file == null || file.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Định dạng file không hợp lệ. Chỉ chấp nhận: JPG, JPEG, PNG, GIF, WEBP");

            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("File quá lớn. Kích thước tối đa là 5MB");

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            
            var fullFolderPath = Path.Combine(webRootPath, folderPath);
            var fullFilePath = Path.Combine(fullFolderPath, fileName);

            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }

        public static void DeleteImage(string? imageFileName, string webRootPath, string folderPath)
        {
            if (string.IsNullOrEmpty(imageFileName))
                return;

            var fileName = imageFileName;
            if (fileName.Contains('/') || fileName.Contains('\\'))
            {
                fileName = Path.GetFileName(fileName);
            }

            var fullPath = Path.Combine(webRootPath, folderPath, fileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch
                {
                }
            }
        }

        public static string GetSachImagePath(string? imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
                return "/images/sach/placeholder.jpg";

            if (imageFileName.StartsWith("/") || imageFileName.Contains("/"))
            {
                return imageFileName;
            }

            return $"/{SACH_IMAGE_FOLDER}/{imageFileName}";
        }
    }
}
