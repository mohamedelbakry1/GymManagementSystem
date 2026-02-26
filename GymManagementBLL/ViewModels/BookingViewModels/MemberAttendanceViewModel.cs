using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.ViewModels.BookingViewModels
{
    public class MemberAttendanceViewModel
    {
        public int SessionId { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public bool IsAttended { get; set; }
    }
}
