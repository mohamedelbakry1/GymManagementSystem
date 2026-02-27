using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(Expression<Func<Session, bool>>? condition = null)
        {
            if(condition is not null)
                return await _dbContext.Sessions.Include(X => X.SessionTrainer)
                   .Include(X => X.SessionCategory)
                   .Where(condition).ToListAsync();
            else
            return await _dbContext.Sessions.Include(X => X.SessionTrainer)
                   .Include(X => X.SessionCategory)
                   .ToListAsync();
        }
        public async Task<Session?> GetSessionWithTrainerAndCategoryAsync(int SessionId)
        {
            return await _dbContext.Sessions.Include(X => X.SessionTrainer)
                                      .Include(X => X.SessionCategory)
                                      .FirstOrDefaultAsync(X => X.Id == SessionId);
        }

        public async Task<int> GetCountOfBookingsAsync(int SessionId)
        {
            return await _dbContext.Bookings.CountAsync(X => X.SessionId == SessionId);
        }
    }
}
