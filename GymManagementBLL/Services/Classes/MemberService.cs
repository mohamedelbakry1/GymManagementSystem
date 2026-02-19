using AutoMapper;
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
    public class MemberService(IUnitOfWork _unitOfWork, IMapper _mapper) : IMemberService
    {
        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var Members = _unitOfWork.GetRepository<Member>().GetAll();
            if (Members is null || !Members.Any()) return [];

            var MembersViewModel = _mapper.Map<IEnumerable<MemberViewModel>>(Members);

            return MembersViewModel;
        }

        public MemberViewModel? GetMemberDetails(int MemberId)
        {
            var Member = _unitOfWork.GetRepository<Member>().GetById(MemberId);
            if (Member is null) return null;

            var MemberViewModel = _mapper.Map<MemberViewModel>(Member);

            var ActiveMembership = _unitOfWork.GetRepository<Membership>().GetAll(X => X.Id == MemberId && X.Status == "Active").FirstOrDefault();
            if (ActiveMembership is not null)
            {
                var Plan = _unitOfWork.GetRepository<Plan>().GetById(ActiveMembership.PlanId);

                MemberViewModel.PlanName = Plan?.Name;
                MemberViewModel.MembershipStartDate = ActiveMembership.CreatedAt.ToShortDateString();
                MemberViewModel.MembershipEndDate = ActiveMembership.EndDate.ToShortDateString();
            }
            return MemberViewModel;
        }

        public HealthRecordViewModel? GetMemberHealthRecordDetails(int MemberId)
        {
            var MemberHealthRecord = _unitOfWork.GetRepository<HealthRecord>().GetById(MemberId);
            if (MemberHealthRecord is null) return null;

            return new HealthRecordViewModel()
            {
                Height = MemberHealthRecord.Height,
                Weight = MemberHealthRecord.Weight,
                BloodType = MemberHealthRecord.BloodType,
                Note = MemberHealthRecord.Note,
            };
        }

        public bool CreateMember(CreateMemberViewModel createMember)
        {
            try
            {
                if (IsEmailExist(createMember.Email) || IsPhoneExist(createMember.Phone)) return false;

                var Member = _mapper.Map<Member>(createMember);

                _unitOfWork.GetRepository<Member>().Add(Member);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Create Member Failed {ex}");
                return false;
            }
        }

        public UpdateMemberViewModel? GetMemberToUpdate(int MemberId)
        {
            var Member = _unitOfWork.GetRepository<Member>().GetById(MemberId);
            if (Member is null) return null;

            return _mapper.Map<UpdateMemberViewModel>(Member);
        }

        public bool UpdateMemberDetails(int MemberId, UpdateMemberViewModel updateMember)
        {
            var MemberRepo = _unitOfWork.GetRepository<Member>();

            var emailExist = MemberRepo.GetAll(X => X.Email == updateMember.Email && X.Id != MemberId).Any();

            var phoneExist = MemberRepo.GetAll(X => X.Phone == updateMember.Phone && X.Id != MemberId).Any();

            if (emailExist || phoneExist) return false;

            try
            {
                var Member = MemberRepo.GetById(MemberId);
                if (Member is null) return false;

                _mapper.Map(updateMember, Member);

                Member.UpdatedAt = DateTime.Now;

                MemberRepo.Update(Member);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Member Failed {ex}");
                return false;
            }
        }

        public bool RemoveMember(int MemberId)
        {
            var MemberRepo = _unitOfWork.GetRepository<Member>();
            var Member = MemberRepo.GetById(MemberId);
            if(Member is null) return false;

            var SessionIdsInBooking = _unitOfWork.GetRepository<Booking>()
                                 .GetAll(X => X.MemberId == MemberId).Select(X => X.SessionId);
            var HasFutureSessions = _unitOfWork.GetRepository<Session>()
                .GetAll(X => SessionIdsInBooking.Contains(X.Id) && X.StartDate > DateTime.Now).Any();

            if (HasFutureSessions) return false;

            var MembershipRepo = _unitOfWork.GetRepository<Membership>();

            var Memberships = MembershipRepo.GetAll(X => X.MemberId == MemberId);
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
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Remove Member Failed {ex}");
                return false;
            }
        }

        #region Helper Methods
        private bool IsEmailExist(string email)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(X => X.Email == email).Any();
        }

        private bool IsPhoneExist(string phone)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(X => X.Phone == phone).Any();
        }
        #endregion
    }
}
