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
        public async Task<IEnumerable<MembershipViewModel>> GetAllMemberships()
        {
            var Memberships = await _unitOfWork.MembershipRepository.GetAllMembershipsWithPlanAndMemberAsync();

            return _mapper.Map<IEnumerable<MembershipViewModel>>(Memberships);
        }

        public async Task<bool> CreateMembership(CreateMembershipViewModel createMembership)
        {
            if(! await IsMemberExist(createMembership.MemberId)) return false;
            if(! await IsPlanExist(createMembership.PlanId)) return false;

            if(await HasActiveMemberships(createMembership.MemberId)) return false;

            try
            {
                var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(createMembership.PlanId);
                var membership = _mapper.Map<Membership>(createMembership);

                membership.EndDate = membership.CreatedAt.AddDays(Plan!.DurationDays);

                await _unitOfWork.MembershipRepository.AddAsync(membership);

                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Create Membership Failed {ex}");
                return false;
            }

        }

        public async Task<bool> DeleteMembership(int MemberId)
        {
            if (! await IsMemberExist(MemberId)) return false;

            if(!await HasActiveMemberships(MemberId)) return false;

            try
            {
                var Membership = (await _unitOfWork.MembershipRepository
                    .GetAllAsync(X => X.MemberId == MemberId && X.EndDate > DateTime.Now)).FirstOrDefault();

                if (Membership is null) return false;

                _unitOfWork.MembershipRepository.Delete(Membership);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Delete Membership Failed {ex}");
                return false;
            }
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetMemberForDropDown()
        {
            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(Members);
        }

        public async Task<IEnumerable<PlanSelectViewModel>> GetPlanForDropDown()
        {
            var Plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync();
            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(Plans);
        }

        #region Helper Methods

        private async Task<bool> IsMemberExist(int MemberId)
        {
            return await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId) is not null;
        }

        private async Task<bool> IsPlanExist(int PlanId)
        {
            return await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId,X => X.IsActive == true) is not null;
        }

        private async Task<bool> HasActiveMemberships(int MemberId)
        {
            return (await _unitOfWork.MembershipRepository
                    .GetAllAsync(X => X.MemberId == MemberId && X.EndDate > DateTime.Now)).Any();
        }
        #endregion
    }
}
