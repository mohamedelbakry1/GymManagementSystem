using GymManagementBLL.ViewModels.MembershipViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default);
        Task<bool> CreateMembershipAsync(CreateMembershipViewModel createMembership, CancellationToken ct = default);
        Task<bool> DeleteMembershipAsync(int MemberId, CancellationToken ct = default);
        Task<IEnumerable<MemberSelectViewModel>> GetMemberForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<PlanSelectViewModel>> GetPlanForDropDownAsync(CancellationToken ct = default);
    }
}
