using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class MembershipController(IMembershipService _membershipService) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Memberships = await _membershipService.GetAllMembershipsAsync(ct);
            return View(Memberships);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await LoadDropDownAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel createMembership, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Membership Failed To Create";
                await LoadDropDownAsync(ct);
                return View(createMembership);
            }

            var result = await _membershipService.CreateMembershipAsync(createMembership, ct);

            if (result)
            {
                TempData["SuccessMessage"] = "Membership Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Membership Can not be Created, Member have an Active Membership";
                await LoadDropDownAsync(ct);
                return View(createMembership);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Cancel(int MemberId, CancellationToken ct)
        {
            if (MemberId <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member and Plan can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var result = await _membershipService.DeleteMembershipAsync(MemberId, ct);

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

        private async Task LoadDropDownAsync(CancellationToken ct = default)
        {
            var Members = await _membershipService.GetMemberForDropDownAsync(ct);
            ViewBag.Members = new SelectList(Members,"Id","Name");

            var Plans = await _membershipService.GetPlanForDropDownAsync(ct);
            ViewBag.Plans = new SelectList(Plans,"Id","Name");
        }

        #endregion
    }
}
