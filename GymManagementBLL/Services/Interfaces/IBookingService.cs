using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsUpcomingAndOngoingAsync(CancellationToken ct = default);
        Task<IEnumerable<MemberBookingViewModel>> GetAllMembersForUpcomingSessionAsync(int SessionId, CancellationToken ct = default);
        Task<IEnumerable<MemberAttendanceViewModel>> GetAllMembersForOngoingSessionAsync(int SessionId, CancellationToken ct = default);
        Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync(int Id, CancellationToken ct = default);
        Task<bool> CreateBookingAsync(CreateBookingViewModel createBooking, CancellationToken ct = default);
        Task<bool> MarkAttendanceAsync(int SessionId, int MemberId, CancellationToken ct = default);
        Task<bool> CancelBookingAsync(int SessionId, int MemberId, CancellationToken ct = default);
    }
}
