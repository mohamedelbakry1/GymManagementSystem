using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.AttachmentService
{
    public interface IAttachmentService
    {
        string? Upload(string folderName, IFormFile file);
        bool Delete(string folderName, string fileName);
    }
}
