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
    public class PlanService(IUnitOfWork _unitOfWork, IMapper _mapper) : IPlanService
    {
        public async Task<IEnumerable<PlanViewModel>> GetAllPlans()
        {
            var Plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync();
            if (Plans is null || !Plans.Any()) return [];

            return _mapper.Map<IEnumerable<PlanViewModel>>(Plans);
        }

        public async Task<PlanViewModel?> GetPlanDetails(int PlanId)
        {
            var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId);
            if (Plan is null) return null;

            return _mapper.Map<PlanViewModel>(Plan);
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdate(int PlanId)
        {
            var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId);
            if(Plan is null || Plan.IsActive == false || await HasActiveMemberships(PlanId)) return null;

            return _mapper.Map<UpdatePlanViewModel>(Plan);
        }

        public async Task<bool> UpdatePlanDetails(int PlanId, UpdatePlanViewModel updatePlan)
        {
            var PlanRepo = _unitOfWork.GetRepository<Plan>();
            var Plan = await PlanRepo.GetByIdAsync(PlanId);
            if (Plan is null || await HasActiveMemberships(PlanId)) return false;
            try
            {
                _mapper.Map(updatePlan, Plan);
                Plan.UpdatedAt = DateTime.Now;

                PlanRepo.Update(Plan);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Plan Failed {ex}");
                return false;
            }
        }

        public async Task<bool> ToggleStatus(int PlanId)
        {
            var PlanRepo = _unitOfWork.GetRepository<Plan>();
            var Plan = await PlanRepo.GetByIdAsync(PlanId);
            if(Plan is null || await HasActiveMemberships(PlanId)) return false;
            try
            {
                Plan.IsActive = Plan.IsActive == true ? false : true;
                Plan.UpdatedAt = DateTime.Now;
                PlanRepo.Update(Plan);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Change Plan Status Failed {ex}");
                return false; 
            }
        }

        #region Helper Methods
        private async Task<bool> HasActiveMemberships(int PlanId)
        {
            var ActiveMembership = await _unitOfWork.GetRepository<Membership>()
                                              .GetAllAsync(X => X.PlanId == PlanId && X.Status == "Active");
            return ActiveMembership.Any();
        }
        #endregion
    }
}
