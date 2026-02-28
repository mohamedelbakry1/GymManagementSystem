using GymManagementBLL.Services.Classes;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BookingController(IBookingService _bookingService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var Sessions = await _bookingService.GetAllSessionsUpcomingAndOngoing();
            return View(Sessions);
        }

        public async Task<IActionResult> GetMembersForUpcomingSessions(int Id)
        {
            var Members = await _bookingService.GetAllMembersForUpcomingSession(Id);
            return View(Members);
        }

        public async Task<IActionResult> GetMembersForOngoingSessions(int Id)
        {
            var Members = await _bookingService.GetAllMembersForOngoingSession(Id);
            return View(Members);
        }

        public async Task<IActionResult> Create(int Id)
        {
            await LoadMemberForDropDown(Id);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel createBooking)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Booking Failed To Create";
            }

            var result = await _bookingService.CreateBooking(createBooking);

            if (result)
            {
                TempData["SuccessMessage"] = "Booking Created Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Booking Can not be Created";
            }
            return RedirectToAction(nameof(GetMembersForUpcomingSessions), new {id = createBooking.SessionId});
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsAttended(int SessionId, int MemberId)
        {
            var result = await _bookingService.MarkAttendance(SessionId,MemberId);
            if (result)
                TempData["SuccessMessage"] = "Attendance Marked Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Mark Attendance";
            return RedirectToAction(nameof(GetMembersForOngoingSessions), new { Id = SessionId});
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int SessionId, int MemberId)
        {
            var result = await _bookingService.CancelBooking(SessionId, MemberId);
            if (result)
                TempData["SuccessMessage"] = "Booking Cancelled Successfully";
            else
                TempData["ErrorMessage"] = "Cannot cancel this booking";
            return RedirectToAction(nameof(GetMembersForUpcomingSessions), new { Id = SessionId });
        }

        #region Helper Mehtods
        private async Task LoadMemberForDropDown(int Id)
        {
            var Members = await _bookingService.GetMembersForDropDown(Id);
            ViewBag.Members = new SelectList(Members, "Id", "Name");
        }

        #endregion


    }
}
