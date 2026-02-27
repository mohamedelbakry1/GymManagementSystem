using GymManagementBLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlans();
        Task<PlanViewModel?> GetPlanDetails(int PlanId);
        Task<UpdatePlanViewModel?> GetPlanToUpdate(int PlanId);
        Task<bool> UpdatePlanDetails(int PlanId, UpdatePlanViewModel updatePlan);
        Task<bool> ToggleStatus(int PlanId);
    }
}
