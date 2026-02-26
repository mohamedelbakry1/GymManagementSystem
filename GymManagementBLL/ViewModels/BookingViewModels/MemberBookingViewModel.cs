using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.ViewModels.BookingViewModels
{
    public class MemberBookingViewModel
    {
        public int MemberId { get; set; }
        public int SessionId { get; set; }
        public string MemberName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public string BookingDate => $"{StartDate:MMM dd , yyyy}";
    }
}
