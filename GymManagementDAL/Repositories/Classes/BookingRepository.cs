using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly GymDbContext _dbContext;

        public BookingRepository(GymDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<Booking>> GetMembersInSessionAsync(int SessionId)
        {
            return await _dbContext.Bookings.Where(X => X.SessionId == SessionId).Include(X => X.Member).ToListAsync();
        }
    }
}
