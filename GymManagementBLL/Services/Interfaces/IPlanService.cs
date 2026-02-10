using GymManagementBLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IPlanService
    {
        IEnumerable<PlanViewModel> GetAllPlans();
        PlanViewModel? GetPlanDetails(int PlanId);
        UpdatePlanViewModel? GetPlanToUpdate(int PlanId);
        bool UpdatePlanDetails(int PlanId, UpdatePlanViewModel updatePlan);
        bool ToggleStatus(int PlanId);
    }
}
