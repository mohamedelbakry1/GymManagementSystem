using GymManagementBLL.ViewModels.MembershipViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipViewModel>> GetAllMemberships();
        Task<bool> CreateMembership(CreateMembershipViewModel createMembership);
        Task<bool> DeleteMembership(int MemberId);
        Task<IEnumerable<MemberSelectViewModel>> GetMemberForDropDown();
        Task<IEnumerable<PlanSelectViewModel>> GetPlanForDropDown();
    }
}
