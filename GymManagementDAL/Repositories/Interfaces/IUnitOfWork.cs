using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();
        public ISessionRepository SessionRepository { get; }
        public IMembershipRepository MembershipRepository { get; }
        int SaveChanges();
    }
}
