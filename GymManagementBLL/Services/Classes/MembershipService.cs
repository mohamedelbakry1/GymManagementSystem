using AutoMapper;
using AutoMapper.Execution;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MembershipViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Member = GymManagementDAL.Entities.Member;

namespace GymManagementBLL.Services.Classes
{
    public class MembershipService(IUnitOfWork _unitOfWork, IMapper _mapper) : IMembershipService
    {
        public IEnumerable<MembershipViewModel> GetAllMemberships()
        {
            var Memberships = _unitOfWork.MembershipRepository.GetAllMembershipsWithPlanAndMember();

            return _mapper.Map<IEnumerable<MembershipViewModel>>(Memberships);
        }

        public bool CreateMembership(CreateMembershipViewModel createMembership)
        {
            if(!IsMemberExist(createMembership.MemberId)) return false;
            if(!IsPlanExist(createMembership.PlanId)) return false;

            if(HasActiveMemberships(createMembership.MemberId)) return false;

            try
            {
                var Plan = _unitOfWork.GetRepository<Plan>().GetById(createMembership.PlanId);
                var membership = _mapper.Map<Membership>(createMembership);

                membership.EndDate = membership.CreatedAt.AddDays(Plan!.DurationDays);

                _unitOfWork.MembershipRepository.Add(membership);

                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Create Membership Failed {ex}");
                return false;
            }

        }

        public bool DeleteMembership(int MemberId)
        {
            if (!IsMemberExist(MemberId)) return false;

            if(!HasActiveMemberships(MemberId)) return false;

            try
            {
                var Membership = _unitOfWork.MembershipRepository
                    .GetAll(X => X.MemberId == MemberId && X.Status == "Active").FirstOrDefault();

                if (Membership is null) return false;

                _unitOfWork.MembershipRepository.Delete(Membership);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Delete Membership Failed {ex}");
                return false;
            }
        }

        public IEnumerable<MemberSelectViewModel> GetMemberForDropDown()
        {
            var Members = _unitOfWork.GetRepository<Member>().GetAll();

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(Members);
        }

        public IEnumerable<PlanSelectViewModel> GetPlanForDropDown()
        {
            var Plans = _unitOfWork.GetRepository<Plan>().GetAll();
            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(Plans);
        }

        #region Helper Methods

        private bool IsMemberExist(int MemberId)
        {
            return _unitOfWork.GetRepository<Member>().GetById(MemberId) is not null;
        }

        private bool IsPlanExist(int PlanId)
        {
            return _unitOfWork.GetRepository<Plan>().GetById(PlanId,X => X.IsActive == true) is not null;
        }

        private bool HasActiveMemberships(int MemberId)
        {
            return _unitOfWork.MembershipRepository
                    .GetAll(X => X.MemberId == MemberId && X.Status == "Active").Any();
        }
        #endregion
    }
}
