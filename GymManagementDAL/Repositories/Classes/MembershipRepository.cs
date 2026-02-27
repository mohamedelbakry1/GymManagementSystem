using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Repositories.Classes
{
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
        private readonly GymDbContext _dbContext;

        public MembershipRepository(GymDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IEnumerable<Membership>> GetAllMembershipsWithPlanAndMemberAsync()
        {
            return await _dbContext.Memberships.Include(M => M.Plan).Include(M => M.Member).ToListAsync();
        }

        


    }
}
