using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Entities
{
    public class Member : GymUser
    {
        public string Photo { get; set; } = null!;

        #region Member - HealthRecord
        public HealthRecord HealthRecord { get; set; } = null!;
        #endregion

        #region Member - Membership
        public ICollection<Membership> Memberships { get; set; } = null!;
        #endregion

        #region Member - Bookings
        public ICollection<Booking> Bookings { get; set; } = null!;
        #endregion

    }
}
