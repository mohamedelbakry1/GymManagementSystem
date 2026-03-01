using GymManagementBLL.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(CreateUserViewModel model);
        Task<bool> DeleteUser(string userId);
        Task<IEnumerable<string>> GetAllRolesAsync();
    }
}
