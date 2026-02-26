using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public IEnumerable<Session> GetAllSessionsWithTrainerAndCategory(Func<Session,bool>? condition = null)
        {
            if(condition is not null)
                return _dbContext.Sessions.Include(X => X.SessionTrainer)
                   .Include(X => X.SessionCategory)
                   .ToList().Where(condition);
            else
            return _dbContext.Sessions.Include(X => X.SessionTrainer)
                   .Include(X => X.SessionCategory)
                   .ToList();
        }
        public Session? GetSessionWithTrainerAndCategory(int SessionId)
        {
            return _dbContext.Sessions.Include(X => X.SessionTrainer)
                                      .Include(X => X.SessionCategory)
                                      .FirstOrDefault(X => X.Id == SessionId);
        }

        public int GetCountOfBookings(int SessionId)
        {
            return _dbContext.Bookings.Count(X => X.SessionId == SessionId);
        }
    }
}
