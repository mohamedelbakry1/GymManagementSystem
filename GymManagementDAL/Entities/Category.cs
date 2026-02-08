using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = null!;

        #region Category - Session
        public ICollection<Session> Sessions { get; set; } = null!;
        #endregion
    }
}
