using GymManagementBLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
        Task<bool> CreateMemberAsync(CreateMemberViewModel createMember, CancellationToken ct = default);
        Task<MemberViewModel?> GetMemberDetailsAsync(int MemberId, CancellationToken ct = default);
        Task<HealthRecordViewModel?> GetMemberHealthRecordDetailsAsync(int MemberId, CancellationToken ct = default);
        Task<UpdateMemberViewModel?>  GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default);
        Task<bool> UpdateMemberDetailsAsync(int MemberId, UpdateMemberViewModel updateMember, CancellationToken ct = default);
        Task<bool> RemoveMemberAsync(int MemberId, CancellationToken ct = default);
    }
}
