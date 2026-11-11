using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null, CancellationToken ct = default);
        Task<TEntity?> GetByIdAsync(int id, Expression<Func<TEntity, bool>>? condition = null, CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null, CancellationToken ct = default);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
