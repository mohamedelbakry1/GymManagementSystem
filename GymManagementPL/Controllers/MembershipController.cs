using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class MembershipController(IMembershipService _membershipService) : Controller
    {
        public IActionResult Index()
        {
            var Memberships = _membershipService.GetAllMemberships();
            return View(Memberships);
        }

        public IActionResult Create()
        {
            LoadDropDown();
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateMembershipViewModel createMembership)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Membership Failed To Create";
                LoadDropDown();
                return View(createMembership);
            }

            var result = _membershipService.CreateMembership(createMembership);

            if (result)
            {
                TempData["SuccessMessage"] = "Membership Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Membership Can not be Created, Member have an Active Membership";
                LoadDropDown();
                return View(createMembership);
            }
        }


        [HttpPost]
        public IActionResult Cancel(int MemberId)
        {
            if (MemberId <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member and Plan can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var result = _membershipService.DeleteMembership(MemberId);

            if (result)
            {
                TempData["SuccessMessage"] = "Membership Canceled Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Membership Can not be Canceled, Member have no Active Membership";
            }
            return RedirectToAction(nameof(Index));
        }


        #region Helper Methods

        private void LoadDropDown()
        {
            var Members = _membershipService.GetMemberForDropDown();
            ViewBag.Members = new SelectList(Members,"Id","Name");

            var Plans = _membershipService.GetPlanForDropDown();
            ViewBag.Plans = new SelectList(Plans,"Id","Name");
        }

        #endregion
    }
}
