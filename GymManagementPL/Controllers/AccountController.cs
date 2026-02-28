using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AccountViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class AccountController(IAccountService _accountService, SignInManager<AppUser> _signInManager) : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var User = await _accountService.ValidateUser(model);
            if (User is null)
            {
                ModelState.AddModelError("InvalidLogin", "Check Email or Password");
                return View(model);
            }

            var Result = await _signInManager.PasswordSignInAsync(User, model.Password,model.RememberMe,false);

            if (Result.IsNotAllowed)
                ModelState.AddModelError("InvalidLogin", "Your Account is not Allowed");
            if(Result.IsLockedOut)
                ModelState.AddModelError("InvalidLogin", "Your Account is Locked Out");
            if(Result.Succeeded)
                return RedirectToAction("Index", "Home");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
