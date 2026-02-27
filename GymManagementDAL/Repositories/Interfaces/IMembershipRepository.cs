using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface IMembershipRepository : IGenericRepository<Membership>
    {
        Task<IEnumerable<Membership>> GetAllMembershipsWithPlanAndMemberAsync();
    }
}
