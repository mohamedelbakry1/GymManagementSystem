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
    public class MemberService(IUnitOfWork _unitOfWork) : IMemberService
    {
        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var Members = _unitOfWork.GetRepository<Member>().GetAll();
            if (Members is null || !Members.Any()) return [];

            var MembersViewModel = Members.Select(X => new MemberViewModel()
            {
                Id = X.Id,
                Name = X.Name,
                Email = X.Email,
                Phone = X.Phone,
                Photo = X.Photo,
                Gender = X.Gender.ToString()
            });

            return MembersViewModel;
        }

        public MemberViewModel? GetMemberDetails(int MemberId)
        {
            var Member = _unitOfWork.GetRepository<Member>().GetById(MemberId);
            if (Member is null) return null;

            var MemberViewModel = new MemberViewModel()
            {
                Name = Member.Name,
                Email = Member.Email,
                Phone = Member.Phone,
                Photo = Member.Photo,
                Gender = Member.Gender.ToString(),
                DateOfBirth = Member.DateOfBirth.ToShortDateString(),
                Address = $"{Member.Address.BuildingNumber}  - {Member.Address.Street} - {Member.Address.City}"
            };

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

                var Member = new Member()
                {
                    Name = createMember.Name,
                    Email = createMember.Email,
                    Phone = createMember.Phone,
                    DateOfBirth = createMember.DateOfBirth,
                    Gender = createMember.Gender,
                    Address = new Address()
                    {
                        BuildingNumber = createMember.BuildingNumber,
                        Street = createMember.Street,
                        City = createMember.City,
                    },
                    HealthRecord = new HealthRecord()
                    {
                        Height = createMember.HealthRecordViewModel.Height,
                        Weight = createMember.HealthRecordViewModel.Weight,
                        BloodType = createMember.HealthRecordViewModel.BloodType,
                        Note = createMember.HealthRecordViewModel.Note,
                    }
                };

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

            return new UpdateMemberViewModel()
            {
                Name = Member.Name,
                Email = Member.Email,
                Phone = Member.Phone,
                Photo = Member.Photo,
                BuildingNumber = Member.Address.BuildingNumber,
                Street = Member.Address.Street,
                City = Member.Address.City,
            };
        }

        public bool UpdateMemberDetails(int MemberId, UpdateMemberViewModel updateMember)
        {
            var MemberRepo = _unitOfWork.GetRepository<Member>();
            try
            {
                var Member = MemberRepo.GetById(MemberId);
                if (Member is null) return false;

                Member.Email = updateMember.Email;
                Member.Phone = updateMember.Phone;
                Member.Address.BuildingNumber = updateMember.BuildingNumber;
                Member.Address.Street = updateMember.Street;
                Member.Address.City = updateMember.City;
                Member.UpdatedAt = DateTime.Now;

                if (IsEmailExist(updateMember.Email) || IsPhoneExist(updateMember.Phone)) return false;

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

            var HasActiveBookings = _unitOfWork.GetRepository<Booking>()
                                 .GetAll(X => X.MemberId == MemberId && X.Session.StartDate > DateTime.Now).Any();
            if (HasActiveBookings) return false;

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
