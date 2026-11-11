using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.AttachmentService
{
    public interface IAttachmentService
    {
        Task<string?> Upload(string folderName, IFormFile file, CancellationToken ct = default);
        bool Delete(string folderName, string fileName);
    }
}
