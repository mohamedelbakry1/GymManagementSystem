using GymManagementBLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMemberService
    {
        IEnumerable<MemberViewModel> GetAllMembers();
        bool CreateMember(CreateMemberViewModel createMember);
        MemberViewModel? GetMemberDetails(int MemberId);
        HealthRecordViewModel? GetMemberHealthRecordDetails(int MemberId);
        UpdateMemberViewModel?  GetMemberToUpdate(int MemberId);
        bool UpdateMemberDetails(int MemberId, UpdateMemberViewModel updateMember);
        bool RemoveMember(int MemberId);
    }
}
