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
    internal class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAndBookingsAsync(Expression<Func<Session, bool>>? condition = null, CancellationToken ct = default)
        {
            if(condition is not null)
                return await _dbContext.Sessions.Include(X => X.SessionTrainer)
                   .Include(X => X.SessionCategory)
                   .Include(X => X.Bookings)
                   .Where(condition).ToListAsync(ct);
            else
            return await _dbContext.Sessions.Include(X => X.SessionTrainer)
                   .Include(X => X.SessionCategory)
                   .Include(X => X.Bookings)
                   .ToListAsync(ct);
        }
        public async Task<Session?> GetSessionWithTrainerAndCategoryAndBookingsAsync(int SessionId, CancellationToken ct = default)
        {
            return await _dbContext.Sessions.Include(X => X.SessionTrainer)
                                      .Include(X => X.SessionCategory)
                                      .Include(X => X.Bookings)
                                      .FirstOrDefaultAsync(X => X.Id == SessionId, ct);
        }

        public async Task<int> GetCountOfBookingsAsync(int SessionId, CancellationToken ct = default)
        {
            return await _dbContext.Bookings.CountAsync(X => X.SessionId == SessionId, ct);
        }
    }
}
