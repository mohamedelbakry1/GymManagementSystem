using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsUpcomingAndOngoing();
        Task<IEnumerable<MemberBookingViewModel>> GetAllMembersForUpcomingSession(int SessionId);
        Task<IEnumerable<MemberAttendanceViewModel>> GetAllMembersForOngoingSession(int SessionId);
        Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDown(int Id);
        Task<bool> CreateBooking(CreateBookingViewModel createBooking);
        Task<bool> MarkAttendance(int SessionId, int MemberId);
        Task<bool> CancelBooking(int SessionId, int MemberId);
    }
}
