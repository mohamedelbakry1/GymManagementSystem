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
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Sessions = await _bookingService.GetAllSessionsUpcomingAndOngoingAsync(ct);
            return View(Sessions);
        }

        public async Task<IActionResult> GetMembersForUpcomingSessions(int Id, CancellationToken ct)
        {
            var Members = await _bookingService.GetAllMembersForUpcomingSessionAsync(Id, ct);
            return View(Members);
        }

        public async Task<IActionResult> GetMembersForOngoingSessions(int Id, CancellationToken ct)
        {
            var Members = await _bookingService.GetAllMembersForOngoingSessionAsync(Id, ct);
            return View(Members);
        }

        public async Task<IActionResult> Create(int Id, CancellationToken ct)
        {
            await LoadMemberForDropDown(Id, ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel createBooking, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Booking Failed To Create";
            }

            var result = await _bookingService.CreateBookingAsync(createBooking, ct);

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
        public async Task<IActionResult> MarkAsAttended(int SessionId, int MemberId, CancellationToken ct)
        {
            var result = await _bookingService.MarkAttendanceAsync(SessionId, MemberId, ct);
            if (result)
                TempData["SuccessMessage"] = "Attendance Marked Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Mark Attendance";
            return RedirectToAction(nameof(GetMembersForOngoingSessions), new { Id = SessionId});
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int SessionId, int MemberId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBookingAsync(SessionId, MemberId, ct);
            if (result)
                TempData["SuccessMessage"] = "Booking Cancelled Successfully";
            else
                TempData["ErrorMessage"] = "Cannot cancel this booking";
            return RedirectToAction(nameof(GetMembersForUpcomingSessions), new { Id = SessionId });
        }

        #region Helper Mehtods
        private async Task LoadMemberForDropDown(int Id, CancellationToken ct = default)
        {
            var Members = await _bookingService.GetMembersForDropDownAsync(Id, ct);
            ViewBag.Members = new SelectList(Members, "Id", "Name");
        }

        #endregion


    }
}
