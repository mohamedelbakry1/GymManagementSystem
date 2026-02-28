using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AccountViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    public class AccountService(UserManager<AppUser> _userManager) : IAccountService
    {
        public async Task<AppUser?> ValidateUser(LoginViewModel login)
        {
            var User = await _userManager.FindByEmailAsync(login.Email);
            if (User is null) return null;
            var IsPasswordValid = await _userManager.CheckPasswordAsync(User, login.Password);
            return IsPasswordValid ? User : null;
        }
    }
}
