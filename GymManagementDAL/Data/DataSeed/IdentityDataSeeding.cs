using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Data.DataSeed
{
    public static class IdentityDataSeeding
    {
        public static async Task<bool> SeedData(UserManager<AppUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            try
            {
                var HasRoles = _roleManager.Roles.Any();
                var HasUsers = _userManager.Users.Any();

                if (HasRoles && HasUsers) return false;

                if (!HasRoles)
                {
                    var Roles = new List<IdentityRole>()
                    {
                        new("SuperAdmin"),
                        new("Admin")
                    };
                    
                    foreach(var role in Roles)
                    {
                        if(! await _roleManager.RoleExistsAsync(role.Name!))
                            await _roleManager.CreateAsync(role);
                    }
                }

                if (!HasUsers)
                {
                    var SuperAdmin = new AppUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Ahmed",
                        Email = "MohamedAhmed@gmail.com",
                        UserName = "MohamedAhmed",
                        PhoneNumber = "01552013062"
                    };

                    await _userManager.CreateAsync(SuperAdmin, "Mohamed@123");
                    await _userManager.AddToRoleAsync(SuperAdmin, "SuperAdmin");

                    var Admin = new AppUser()
                    {
                        FirstName = "Osama",
                        LastName = "Mohamed",
                        Email = "OsamaMohamed@gmail.com",
                        UserName = "OsamaMohamed",
                        PhoneNumber = "01015594223"
                    };
                    await _userManager.CreateAsync(Admin, "Osama@123");
                    await _userManager.AddToRoleAsync(Admin, "Admin");
                }
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed To Seed Data:{ex}");
                return false;
            }
        }
    }
}
