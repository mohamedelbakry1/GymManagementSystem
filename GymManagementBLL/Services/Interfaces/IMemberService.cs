using GymManagementBLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembers();
        Task<bool> CreateMember(CreateMemberViewModel createMember);
        Task<MemberViewModel?> GetMemberDetails(int MemberId);
        Task<HealthRecordViewModel?> GetMemberHealthRecordDetails(int MemberId);
        Task<UpdateMemberViewModel?>  GetMemberToUpdate(int MemberId);
        Task<bool> UpdateMemberDetails(int MemberId, UpdateMemberViewModel updateMember);
        Task<bool> RemoveMember(int MemberId);
    }
}
