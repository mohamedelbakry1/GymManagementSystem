using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    internal class PlanService(IUnitOfWork _unitOfWork, IMapper _mapper) : IPlanService
    {
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var Plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            if (Plans is null || !Plans.Any()) return [];

            return _mapper.Map<IEnumerable<PlanViewModel>>(Plans);
        }

        public async Task<PlanViewModel?> GetPlanDetailsAsync(int PlanId, CancellationToken ct = default)
        {
            var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct: ct);
            if (Plan is null) return null;

            return _mapper.Map<PlanViewModel>(Plan);
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int PlanId, CancellationToken ct = default)
        {
            var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct: ct);
            if(Plan is null || Plan.IsActive == false || await HasActiveMembershipsAsync(PlanId, ct)) return null;

            return _mapper.Map<UpdatePlanViewModel>(Plan);
        }

        public async Task<bool> UpdatePlanDetailsAsync(int PlanId, UpdatePlanViewModel updatePlan, CancellationToken ct = default)
        {
            var PlanRepo = _unitOfWork.GetRepository<Plan>();
            var Plan = await PlanRepo.GetByIdAsync(PlanId, ct: ct);
            if (Plan is null || await HasActiveMembershipsAsync(PlanId, ct)) return false;
            try
            {
                _mapper.Map(updatePlan, Plan);
                Plan.UpdatedAt = DateTime.Now;

                PlanRepo.Update(Plan);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Plan Failed {ex}");
                return false;
            }
        }

        public async Task<bool> ToggleStatusAsync(int PlanId, CancellationToken ct = default)
        {
            var PlanRepo = _unitOfWork.GetRepository<Plan>();
            var Plan = await PlanRepo.GetByIdAsync(PlanId, ct: ct);
            if(Plan is null || await HasActiveMembershipsAsync(PlanId, ct)) return false;
            try
            {
                Plan.IsActive = Plan.IsActive == true ? false : true;
                Plan.UpdatedAt = DateTime.Now;
                PlanRepo.Update(Plan);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Change Plan Status Failed {ex}");
                return false; 
            }
        }

        #region Helper Methods
        private async Task<bool> HasActiveMembershipsAsync(int PlanId, CancellationToken ct = default)
        {
            var ActiveMembership = await _unitOfWork.GetRepository<Membership>()
                                              .GetAllAsync(X => X.PlanId == PlanId && X.EndDate > DateTime.Now && X.CreatedAt < DateTime.Now, ct);
            return ActiveMembership.Any();
        }
        #endregion
    }
}
