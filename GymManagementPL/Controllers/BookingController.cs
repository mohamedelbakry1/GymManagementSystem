using GymManagementBLL.Services.Classes;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class BookingController(IBookingService _bookingService) : Controller
    {
        public IActionResult Index()
        {
            var Sessions = _bookingService.GetAllSessionsUpcomingAndOngoing();
            return View(Sessions);
        }

        public IActionResult GetMembersForUpcomingSessions(int Id)
        {
            var Members = _bookingService.GetAllMembersForUpcomingSession(Id);
            return View(Members);
        }

        public IActionResult GetMembersForOngoingSessions(int Id)
        {
            var Members = _bookingService.GetAllMembersForOngoingSession(Id);
            return View(Members);
        }

        public IActionResult Create(int Id)
        {
            LoadMemberForDropDown(Id);
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateBookingViewModel createBooking)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Booking Failed To Create";
            }

            var result = _bookingService.CreateBooking(createBooking);

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
        public IActionResult MarkAsAttended(int SessionId, int MemberId)
        {
            var result = _bookingService.MarkAttendance(SessionId,MemberId);
            if (result)
                TempData["SuccessMessage"] = "Attendance Marked Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Mark Attendance";
            return RedirectToAction(nameof(GetMembersForOngoingSessions), new { Id = SessionId});
        }

        [HttpPost]
        public IActionResult Cancel(int SessionId, int MemberId)
        {
            var result = _bookingService.CancelBooking(SessionId, MemberId);
            if (result)
                TempData["SuccessMessage"] = "Booking Cancelled Successfully";
            else
                TempData["ErrorMessage"] = "Cannot cancel this booking";
            return RedirectToAction(nameof(GetMembersForUpcomingSessions), new { Id = SessionId });
        }

        #region Helper Mehtods
        private void LoadMemberForDropDown(int Id)
        {
            var Members = _bookingService.GetMembersForDropDown(Id);
            ViewBag.Members = new SelectList(Members, "Id", "Name");
        }

        #endregion


    }
}
