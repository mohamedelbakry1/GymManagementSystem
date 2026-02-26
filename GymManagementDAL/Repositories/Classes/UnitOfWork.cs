using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Repositories.Classes
{
    public class UnitOfWork(GymDbContext _dbContext, 
        ISessionRepository sessionRepository,
        IMembershipRepository membershipRepository,
        IBookingRepository bookingRepository
        ) : IUnitOfWork
    {
        private readonly ConcurrentDictionary<Type, object> _repositories = new ConcurrentDictionary<Type, object>();

        public ISessionRepository SessionRepository { get; } = sessionRepository;

        public IMembershipRepository MembershipRepository { get; } = membershipRepository;

        public IBookingRepository BookingRepository { get; } = bookingRepository;

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            return (IGenericRepository<TEntity>) _repositories.GetOrAdd(typeof(TEntity),new GenericRepository<TEntity>(_dbContext));
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
