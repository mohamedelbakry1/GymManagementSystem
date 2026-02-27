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
    public class GenericRepository<TEntity>(GymDbContext _dbContext) : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity,bool>>? condition = null)
        {
            if (condition is null)
                return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
            else
                return await _dbContext.Set<TEntity>().AsNoTracking()
                       .Where(condition).ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id, Expression<Func<TEntity, bool>>? condition = null)
        {
            if (condition is not null)
            return await _dbContext.Set<TEntity>().Where(condition).FirstOrDefaultAsync();

            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
    }
}
