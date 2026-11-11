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
    internal class GenericRepository<TEntity>(GymDbContext _dbContext) : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity,bool>>? condition = null, CancellationToken ct = default)
        {
            if (condition is null)
                return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync(ct);
            else
                return await _dbContext.Set<TEntity>().AsNoTracking()
                       .Where(condition).ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(int id, Expression<Func<TEntity, bool>>? condition = null, CancellationToken ct = default)
        {
            if (condition is not null)
            return await _dbContext.Set<TEntity>().Where(condition).FirstOrDefaultAsync(ct);

            return await _dbContext.Set<TEntity>().FindAsync(id, ct);
        }

        public void Add(TEntity entity) => _dbContext.Set<TEntity>().Add(entity);

        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);

        public void Delete(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);

        public Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null, CancellationToken ct = default)
        {
            if(condition is not null)
                return _dbContext.Set<TEntity>().Where(condition).CountAsync(ct);
            return _dbContext.Set<TEntity>().CountAsync(ct);
        }
    }
}
