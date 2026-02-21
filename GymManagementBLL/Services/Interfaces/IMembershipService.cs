using GymManagementBLL.ViewModels.MembershipViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMembershipService
    {
        IEnumerable<MembershipViewModel> GetAllMemberships();
        bool CreateMembership(CreateMembershipViewModel createMembership);
        bool DeleteMembership(int MemberId);
        IEnumerable<MemberSelectViewModel> GetMemberForDropDown();
        IEnumerable<PlanSelectViewModel> GetPlanForDropDown();
    }
}
