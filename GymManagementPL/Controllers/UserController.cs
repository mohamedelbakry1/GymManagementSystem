using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.UserViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UserController(IUserService _userService, UserManager<AppUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var Users = await _userService.GetAllUsersAsync();
            return View(Users);
        }

        public async Task<IActionResult> Create()
        {
            await LoadRolesDropDown();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel createUser)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesDropDown();
                return View(createUser);
            }
            var result = await _userService.CreateUserAsync(createUser);
            if (result)
                TempData["SuccessMessage"] = "User Created Successfully";
            else
                TempData["ErrorMessage"] = "User Failed To Create, Check Email or Selected Role";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                TempData["ErrorMessage"] = "Invalid User Id";
                return RedirectToAction(nameof(Index));
            }

            var Users = await _userService.GetAllUsersAsync();
            var User = Users.FirstOrDefault(U => U.Id == Id);

            if (User is null)
            {
                TempData["ErrorMessage"] = "User Not Found";
                return RedirectToAction(nameof(Index));
            }

            var CurrentUserId = _userManager.GetUserId(HttpContext.User);
            if (CurrentUserId == Id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserId = Id;
            ViewBag.UserFullName = User.FullName;
            ViewBag.UserEmail = User.Email;
            ViewBag.UserRole = User.Role;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string Id)
        {
            var CurrentUserId = _userManager.GetUserId(HttpContext.User);
            if (CurrentUserId == Id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userService.DeleteUser(Id);

            if (result)
                TempData["SuccessMessage"] = "User Deleted Successfully";
            else
                TempData["ErrorMessage"] = "User Failed To Delete";

            return RedirectToAction(nameof(Index));
        }


        #region Helper Methods
        private async Task LoadRolesDropDown()
        {
            var Roles = await _userService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(Roles);
        }
        #endregion
    }
}
