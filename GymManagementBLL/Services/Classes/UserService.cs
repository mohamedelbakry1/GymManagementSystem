using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.UserViewModels;
using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    internal class UserService(UserManager<AppUser> _userManager, RoleManager<IdentityRole> _roleManager, GymDbContext _dbContext) : IUserService
    {
        public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync(CancellationToken ct = default)
        {
            var usersWithRoles = await (from user in _userManager.Users.AsNoTracking()
                                        join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId into ur
                                        from userRole in ur.DefaultIfEmpty()
                                        join role in _roleManager.Roles.AsNoTracking() on userRole.RoleId equals role.Id into r
                                        from role in r.DefaultIfEmpty()
                                        select new UserViewModel
                                        {
                                            Id = user.Id,
                                            FullName = $"{user.FirstName} {user.LastName}",
                                            Email = user.Email ?? string.Empty,
                                            Role = role != null ? role.Name! : "No Role"
                                        }).ToListAsync(ct);
            return usersWithRoles;
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

        public async Task<bool> DeleteUserAsync(string userId)
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
