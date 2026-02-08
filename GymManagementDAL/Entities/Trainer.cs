using GymManagementDAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Entities
{
    public class Trainer : GymUser
    {
        public Specialties Specialties { get; set; }

        #region Trainer - Session
        public ICollection<Session> TrainerSessions { get; set; } = null!;
        #endregion
    }
}
