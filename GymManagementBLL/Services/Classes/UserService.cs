using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.UserViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    public class UserService(UserManager<AppUser> _userManager, RoleManager<IdentityRole> _roleManager) : IUserService
    {
        public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
        {
            var Users = await _userManager.Users.ToListAsync();

            var result = new List<UserViewModel>();

            foreach (var user in Users)
            {
                var Roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email!,
                    Role = Roles.FirstOrDefault() ?? "No Role"
                });
            }
            return result;
        }

        public async Task<bool> CreateUserAsync(CreateUserViewModel model)
        {
            try
            {
                var UserExist = await _userManager.FindByEmailAsync(model.Email);
                if (UserExist is not null) return false;

                var RoleExist = await _roleManager.RoleExistsAsync(model.Role);
                if(!RoleExist) return false;

                var user = new AppUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded) return false;

                await _userManager.AddToRoleAsync(user, model.Role);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create User Failed: {ex}");
                return false;
            }
        }

        public async Task<bool> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null) return false;

                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete User Failed: {ex}");
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(R => R.Name!).ToListAsync();
        }

    }
}
