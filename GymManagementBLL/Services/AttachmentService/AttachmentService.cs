using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.AttachmentService
{
    public class AttachmentService(IWebHostEnvironment _webHost) : IAttachmentService
    {
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly long maxFileSize = 5 * 1024 * 1024;

        public async Task<string?> Upload(string folderName, IFormFile file)
        {
            try
            {
                if (string.IsNullOrEmpty(folderName) || file is null || file.Length == 0) return null;

                if (file.Length > maxFileSize) return null;

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension)) return null;

                var folderPath = Path.Combine(_webHost.WebRootPath, "images", folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + extension;

                var filePath = Path.Combine(folderPath, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);

                return fileName;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed To Upload File To Folder = {folderName} : {ex}");
                return null;
            }
        }
        public bool Delete(string folderName, string fileName)
        {
            try
            {
                if(string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName))
                    return false;

                var FullPath = Path.Combine(_webHost.WebRootPath, "images", folderName, fileName);
                if (File.Exists(FullPath))
                {
                    File.Delete(FullPath);
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed To Delete File with Name {fileName} : {ex}");
                return false;
            }
        }
    }
}
