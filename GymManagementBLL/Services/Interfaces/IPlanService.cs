using GymManagementBLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanDetailsAsync(int PlanId, CancellationToken ct = default);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int PlanId, CancellationToken ct = default);
        Task<bool> UpdatePlanDetailsAsync(int PlanId, UpdatePlanViewModel updatePlan, CancellationToken ct = default);
        Task<bool> ToggleStatusAsync(int PlanId, CancellationToken ct = default);
    }
}
