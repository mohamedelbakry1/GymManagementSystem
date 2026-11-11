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
    internal class MemberService(IUnitOfWork _unitOfWork, IMapper _mapper, IAttachmentService _attachmentService) : IMemberService
    {
        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (Members is null || !Members.Any()) return [];

            var MembersViewModel = _mapper.Map<IEnumerable<MemberViewModel>>(Members);

            return MembersViewModel;
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int MemberId, CancellationToken ct = default)
        {
            var Member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct: ct);
            if (Member is null) return null;

            var MemberViewModel = _mapper.Map<MemberViewModel>(Member);

            var ActiveMembership = (await _unitOfWork.GetRepository<Membership>()
                                    .GetAllAsync(X => X.MemberId == MemberId && X.EndDate > DateTime.Now, ct))
                                    .FirstOrDefault();

            if (ActiveMembership is not null)
            {
                var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(ActiveMembership.PlanId, ct: ct);

                MemberViewModel.PlanName = Plan?.Name;
                MemberViewModel.MembershipStartDate = ActiveMembership.CreatedAt.ToShortDateString();
                MemberViewModel.MembershipEndDate = ActiveMembership.EndDate.ToShortDateString();
            }
            return MemberViewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordDetailsAsync(int MemberId, CancellationToken ct = default)
        {
            var MemberHealthRecord = await _unitOfWork.GetRepository<HealthRecord>().GetByIdAsync(MemberId, ct: ct);
            if (MemberHealthRecord is null) return null;

            return _mapper.Map<HealthRecordViewModel>(MemberHealthRecord);
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel createMember, CancellationToken ct = default)
        {
            try
            {
                if (await IsEmailExistAsync(createMember.Email, ct) || await IsPhoneExistAsync(createMember.Phone, ct)) return false;

                var PhotoName = await _attachmentService.Upload("Members", createMember.PhotoFile);
                if(string.IsNullOrEmpty(PhotoName)) return false;

                var Member = _mapper.Map<Member>(createMember);
                Member.Photo = PhotoName;

                _unitOfWork.GetRepository<Member>().Add(Member);
                var IsCreated = await _unitOfWork.SaveChangesAsync(ct) > 0;

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

        public async Task<UpdateMemberViewModel?> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default)
        {
            var Member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct: ct);
            if (Member is null) return null;

            return _mapper.Map<UpdateMemberViewModel>(Member);
        }

        public async Task<bool> UpdateMemberDetailsAsync(int MemberId, UpdateMemberViewModel updateMember, CancellationToken ct = default)
        {
            var MemberRepo = _unitOfWork.GetRepository<Member>();

            var emailExist = (await MemberRepo.GetAllAsync(X => X.Email == updateMember.Email && X.Id != MemberId, ct)).Any();

            var phoneExist = (await MemberRepo.GetAllAsync(X => X.Phone == updateMember.Phone && X.Id != MemberId, ct)).Any();

            if (emailExist || phoneExist) return false;

            try
            {
                var Member = await MemberRepo.GetByIdAsync(MemberId, ct: ct);
                if (Member is null) return false;

                _mapper.Map(updateMember, Member);

                Member.UpdatedAt = DateTime.Now;

                MemberRepo.Update(Member);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Member Failed {ex}");
                return false;
            }
        }

        public async Task<bool> RemoveMemberAsync(int MemberId, CancellationToken ct = default)
        {
            var MemberRepo = _unitOfWork.GetRepository<Member>();
            var Member = await MemberRepo.GetByIdAsync(MemberId, ct: ct);
            if(Member is null) return false;

            var SessionIdsInBooking = (await _unitOfWork.GetRepository<Booking>()
                                 .GetAllAsync(X => X.MemberId == MemberId, ct)).Select(X => X.SessionId);
            var HasFutureSessions = (await _unitOfWork.GetRepository<Session>()
                .GetAllAsync(X => SessionIdsInBooking.Contains(X.Id) && X.StartDate > DateTime.Now, ct)).Any();

            if (HasFutureSessions) return false;

            var MembershipRepo = _unitOfWork.GetRepository<Membership>();

            var Memberships = await MembershipRepo.GetAllAsync(X => X.MemberId == MemberId, ct);
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
                var IsDeleted = await _unitOfWork.SaveChangesAsync(ct) > 0;
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
        private async Task<bool> IsEmailExistAsync(string email, CancellationToken ct = default)
        {
            return (await _unitOfWork.GetRepository<Member>().GetAllAsync(X => X.Email == email, ct)).Any();
        }

        private async Task<bool> IsPhoneExistAsync(string phone, CancellationToken ct = default)
        {
            return (await _unitOfWork.GetRepository<Member>().GetAllAsync(X => X.Phone == phone, ct)).Any();
        }
        #endregion
    }
}
