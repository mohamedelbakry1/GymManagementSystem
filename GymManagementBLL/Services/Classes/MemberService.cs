using AutoMapper;
using GymManagementBLL.Services.AttachmentService;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class MemberService(IUnitOfWork _unitOfWork, IMapper _mapper, IAttachmentService _attachmentService) : IMemberService
    {
        public async Task<IEnumerable<MemberViewModel>> GetAllMembers()
        {
            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync();
            if (Members is null || !Members.Any()) return [];

            var MembersViewModel = _mapper.Map<IEnumerable<MemberViewModel>>(Members);

            return MembersViewModel;
        }

        public async Task<MemberViewModel?> GetMemberDetails(int MemberId)
        {
            var Member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId);
            if (Member is null) return null;

            var MemberViewModel = _mapper.Map<MemberViewModel>(Member);

            var ActiveMembership = (await _unitOfWork.GetRepository<Membership>()
                                    .GetAllAsync(X => X.Id == MemberId && X.EndDate > DateTime.Now))
                                    .FirstOrDefault();

            if (ActiveMembership is not null)
            {
                var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(ActiveMembership.PlanId);

                MemberViewModel.PlanName = Plan?.Name;
                MemberViewModel.MembershipStartDate = ActiveMembership.CreatedAt.ToShortDateString();
                MemberViewModel.MembershipEndDate = ActiveMembership.EndDate.ToShortDateString();
            }
            return MemberViewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordDetails(int MemberId)
        {
            var MemberHealthRecord = await _unitOfWork.GetRepository<HealthRecord>().GetByIdAsync(MemberId);
            if (MemberHealthRecord is null) return null;

            return _mapper.Map<HealthRecordViewModel>(MemberHealthRecord);
        }

        public async Task<bool> CreateMember(CreateMemberViewModel createMember)
        {
            try
            {
                if (await IsEmailExist(createMember.Email) || await IsPhoneExist(createMember.Phone)) return false;

                var PhotoName = await _attachmentService.Upload("Members", createMember.PhotoFile);
                if(string.IsNullOrEmpty(PhotoName)) return false;

                var Member = _mapper.Map<Member>(createMember);
                Member.Photo = PhotoName;

                await _unitOfWork.GetRepository<Member>().AddAsync(Member);
                var IsCreated = await _unitOfWork.SaveChangesAsync() > 0;

                if (!IsCreated)
                {
                    _attachmentService.Delete("Members", PhotoName);
                    return false;
                }
                return IsCreated;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Create Member Failed {ex}");
                return false;
            }
        }

        public async Task<UpdateMemberViewModel?> GetMemberToUpdate(int MemberId)
        {
            var Member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId);
            if (Member is null) return null;

            return _mapper.Map<UpdateMemberViewModel>(Member);
        }

        public async Task<bool> UpdateMemberDetails(int MemberId, UpdateMemberViewModel updateMember)
        {
            var MemberRepo = _unitOfWork.GetRepository<Member>();

            var emailExist = (await MemberRepo.GetAllAsync(X => X.Email == updateMember.Email && X.Id != MemberId)).Any();

            var phoneExist = (await MemberRepo.GetAllAsync(X => X.Phone == updateMember.Phone && X.Id != MemberId)).Any();

            if (emailExist || phoneExist) return false;

            try
            {
                var Member = await MemberRepo.GetByIdAsync(MemberId);
                if (Member is null) return false;

                _mapper.Map(updateMember, Member);

                Member.UpdatedAt = DateTime.Now;

                MemberRepo.Update(Member);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Member Failed {ex}");
                return false;
            }
        }

        public async Task<bool> RemoveMember(int MemberId)
        {
            var MemberRepo = _unitOfWork.GetRepository<Member>();
            var Member = await MemberRepo.GetByIdAsync(MemberId);
            if(Member is null) return false;

            var SessionIdsInBooking = (await _unitOfWork.GetRepository<Booking>()
                                 .GetAllAsync(X => X.MemberId == MemberId)).Select(X => X.SessionId);
            var HasFutureSessions = (await _unitOfWork.GetRepository<Session>()
                .GetAllAsync(X => SessionIdsInBooking.Contains(X.Id) && X.StartDate > DateTime.Now)).Any();

            if (HasFutureSessions) return false;

            var MembershipRepo = _unitOfWork.GetRepository<Membership>();

            var Memberships = await MembershipRepo.GetAllAsync(X => X.MemberId == MemberId);
            try
            {
                if (Memberships.Any())
                {
                    foreach(var Membership in Memberships)      
                    {
                        MembershipRepo.Delete(Membership);
                    }
                }
                MemberRepo.Delete(Member);
                var IsDeleted = await _unitOfWork.SaveChangesAsync() > 0;
                if (IsDeleted)
                    _attachmentService.Delete("Members", Member.Photo);
                    
                return IsDeleted;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Remove Member Failed {ex}");
                return false;
            }
        }

        #region Helper Methods
        private async Task<bool> IsEmailExist(string email)
        {
            return (await _unitOfWork.GetRepository<Member>().GetAllAsync(X => X.Email == email)).Any();
        }

        private async Task<bool> IsPhoneExist(string phone)
        {
            return (await _unitOfWork.GetRepository<Member>().GetAllAsync(X => X.Phone == phone)).Any();
        }
        #endregion
    }
}
