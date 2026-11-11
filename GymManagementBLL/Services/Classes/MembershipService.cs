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
    internal class MembershipService(IUnitOfWork _unitOfWork, IMapper _mapper) : IMembershipService
    {
        public async Task<IEnumerable<MembershipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default)
        {
            var Memberships = await _unitOfWork.MembershipRepository.GetAllMembershipsWithPlanAndMemberAsync(ct);

            return _mapper.Map<IEnumerable<MembershipViewModel>>(Memberships);
        }

        public async Task<bool> CreateMembershipAsync(CreateMembershipViewModel createMembership, CancellationToken ct = default)
        {
            if(! await IsMemberExistAsync(createMembership.MemberId, ct)) return false;
            if(! await IsPlanExistAsync(createMembership.PlanId, ct)) return false;

            if(await HasActiveMembershipsAsync(createMembership.MemberId, ct)) return false;

            try
            {
                var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(createMembership.PlanId, ct: ct);
                var membership = _mapper.Map<Membership>(createMembership);

                membership.EndDate = DateTime.Now.AddDays(Plan!.DurationDays);

                _unitOfWork.MembershipRepository.Add(membership);

                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Create Membership Failed {ex}");
                return false;
            }

        }

        public async Task<bool> DeleteMembershipAsync(int MemberId, CancellationToken ct = default)
        {
            if (! await IsMemberExistAsync(MemberId, ct)) return false;

            if(!await HasActiveMembershipsAsync(MemberId, ct)) return false;

            try
            {
                var Membership = (await _unitOfWork.MembershipRepository
                    .GetAllAsync(X => X.MemberId == MemberId && X.EndDate > DateTime.Now, ct)).FirstOrDefault();

                if (Membership is null) return false;

                _unitOfWork.MembershipRepository.Delete(Membership);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Delete Membership Failed {ex}");
                return false;
            }
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetMemberForDropDownAsync(CancellationToken ct = default)
        {
            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(Members);
        }

        public async Task<IEnumerable<PlanSelectViewModel>> GetPlanForDropDownAsync(CancellationToken ct = default)
        {
            var Plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(Plans);
        }

        #region Helper Methods

        private async Task<bool> IsMemberExistAsync(int MemberId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct: ct) is not null;
        }

        private async Task<bool> IsPlanExistAsync(int PlanId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId,X => X.IsActive == true, ct: ct) is not null;
        }

        private async Task<bool> HasActiveMembershipsAsync(int MemberId, CancellationToken ct = default)
        {
            return (await _unitOfWork.MembershipRepository
                    .GetAllAsync(X => X.MemberId == MemberId && X.EndDate > DateTime.Now, ct)).Any();
        }
        #endregion
    }
}
