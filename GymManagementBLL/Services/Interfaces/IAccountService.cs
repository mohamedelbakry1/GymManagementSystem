using GymManagementBLL.ViewModels.AccountViewModels;
using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AppUser?> ValidateUserAsync(LoginViewModel login);
    }
}
