using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MemberController(IMemberService _memberService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var Members = await _memberService.GetAllMembers();
            return View(Members);
        }

        public async Task<IActionResult> MemberDetails(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Member = await _memberService.GetMemberDetails(Id);

            if (Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(Member);
        }

        public async Task<IActionResult> HealthRecordDetails(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var HealthRecord = await _memberService.GetMemberHealthRecordDetails(Id);

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
        public async Task<IActionResult> CreateMember(CreateMemberViewModel createMember)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataInvalid","Check Data and Missing Fields");
                return RedirectToAction(nameof(Create), createMember);
            }

            var result = await _memberService.CreateMember(createMember);

            if (result)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Member Failed To Create, Check Phone and Email";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MemberEdit(int Id)
        {
            if(Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }
            var Member = await _memberService.GetMemberToUpdate(Id);
            if(Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(Member);
        }

        [HttpPost]
        public async Task<IActionResult> MemberEdit([FromRoute] int Id, UpdateMemberViewModel updateMember)
        {
            if (!ModelState.IsValid)
                return View(updateMember);

            var result = await _memberService.UpdateMemberDetails(Id, updateMember);

            if (result)
                TempData["SuccessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = "Member Failed To Update";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Member can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }
            var Member = await _memberService.GetMemberDetails(Id);
            if(Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MemberId = Id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int Id)
        {
            var result = await _memberService.RemoveMember(Id);

            if (result)
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Member Can not Delete";

            return RedirectToAction(nameof(Index));
        }

    }
}
