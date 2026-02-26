using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        IEnumerable<Session> GetAllSessionsWithTrainerAndCategory(Func<Session, bool>? condition = null);
        Session? GetSessionWithTrainerAndCategory(int SessionId);
        int GetCountOfBookings(int SessionId);
    }
}
