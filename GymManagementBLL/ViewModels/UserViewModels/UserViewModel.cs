using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.ViewModels.UserViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
