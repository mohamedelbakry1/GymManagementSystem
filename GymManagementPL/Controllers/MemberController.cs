using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MemberController(IMemberService _memberService) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Members = await _memberService.GetAllMembersAsync(ct);
            return View(Members);
        }

        public async Task<IActionResult> MemberDetails(int Id, CancellationToken ct)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Member = await _memberService.GetMemberDetailsAsync(Id, ct);

            if (Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(Member);
        }

        public async Task<IActionResult> HealthRecordDetails(int Id, CancellationToken ct)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var HealthRecord = await _memberService.GetMemberHealthRecordDetailsAsync(Id, ct);

            if(HealthRecord is null)
            {
                TempData["ErrorMessage"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }
            
            return View(HealthRecord);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel createMember, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataInvalid","Check Data and Missing Fields");
                return RedirectToAction(nameof(Create), createMember);
            }

            var result = await _memberService.CreateMemberAsync(createMember, ct);

            if (result)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Member Failed To Create, Check Phone and Email";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MemberEdit(int Id, CancellationToken ct)
        {
            if(Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }
            var Member = await _memberService.GetMemberToUpdateAsync(Id, ct);
            if(Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(Member);
        }

        [HttpPost]
        public async Task<IActionResult> MemberEdit([FromRoute] int Id, UpdateMemberViewModel updateMember, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(updateMember);

            var result = await _memberService.UpdateMemberDetailsAsync(Id, updateMember, ct);

            if (result)
                TempData["SuccessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = "Member Failed To Update";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id, CancellationToken ct)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }
            var Member = await _memberService.GetMemberDetailsAsync(Id, ct);
            if(Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MemberId = Id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int Id, CancellationToken ct)
        {
            var result = await _memberService.RemoveMemberAsync(Id, ct);

            if (result)
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Member Can not Delete";

            return RedirectToAction(nameof(Index));
        }

    }
}
