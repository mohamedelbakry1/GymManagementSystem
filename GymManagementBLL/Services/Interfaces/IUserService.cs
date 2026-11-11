using GymManagementBLL.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllUsersAsync(CancellationToken ct = default);
        Task<bool> CreateUserAsync(CreateUserViewModel model);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<string>> GetAllRolesAsync();
    }
}
