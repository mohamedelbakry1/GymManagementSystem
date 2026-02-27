using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(Expression<Func<Session, bool>>? condition = null);
        Task<Session?> GetSessionWithTrainerAndCategoryAsync(int SessionId);
        Task<int> GetCountOfBookingsAsync(int SessionId);
    }
}
