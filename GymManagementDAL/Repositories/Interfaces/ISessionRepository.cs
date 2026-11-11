using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAndBookingsAsync(Expression<Func<Session, bool>>? condition = null, CancellationToken ct = default);
        Task<Session?> GetSessionWithTrainerAndCategoryAndBookingsAsync(int SessionId, CancellationToken ct = default);
        Task<int> GetCountOfBookingsAsync(int SessionId, CancellationToken ct = default);
    }
}
