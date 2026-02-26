using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IBookingService
    {
        IEnumerable<SessionViewModel> GetAllSessionsUpcomingAndOngoing();
        IEnumerable<MemberBookingViewModel> GetAllMembersForUpcomingSession(int SessionId);
        IEnumerable<MemberAttendanceViewModel> GetAllMembersForOngoingSession(int SessionId);
        IEnumerable<MemberSelectViewModel> GetMembersForDropDown(int Id);
        bool CreateBooking(CreateBookingViewModel createBooking);
        bool MarkAttendance(int SessionId, int MemberId);
        bool CancelBooking(int SessionId, int MemberId);
    }
}
