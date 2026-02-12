using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    public class PlanService(IUnitOfWork _unitOfWork) : IPlanService
    {
        public IEnumerable<PlanViewModel> GetAllPlans()
        {
            var Plans = _unitOfWork.GetRepository<Plan>().GetAll();
            if (Plans is null || !Plans.Any()) return [];

            return Plans.Select(P => new PlanViewModel()
            {
                Id = P.Id,
                Name = P.Name,
                Description = P.Description,
                Price = P.Price,
                DurationDays = P.DurationDays,
                IsActive = P.IsActive,
            });
        }

        public PlanViewModel? GetPlanDetails(int PlanId)
        {
            var Plan = _unitOfWork.GetRepository<Plan>().GetById(PlanId);
            if (Plan is null) return null;

            return new PlanViewModel()
            {
                Name = Plan.Name,
                Description = Plan.Description,
                Price = Plan.Price,
                DurationDays = Plan.DurationDays,
                IsActive = Plan.IsActive,
            };
        }

        public UpdatePlanViewModel? GetPlanToUpdate(int PlanId)
        {
            var Plan = _unitOfWork.GetRepository<Plan>().GetById(PlanId);
            if(Plan is null || Plan.IsActive == false || HasActiveMemberships(PlanId)) return null;

            return new UpdatePlanViewModel()
            {
                Name = Plan.Name,
                Description = Plan.Description,
                Price = Plan.Price,
                DurationDays = Plan.DurationDays,
            };
        }

        public bool UpdatePlanDetails(int PlanId, UpdatePlanViewModel updatePlan)
        {
            var PlanRepo = _unitOfWork.GetRepository<Plan>();
            var Plan = PlanRepo.GetById(PlanId);
            if (Plan is null || HasActiveMemberships(PlanId)) return false;
            try
            {
                (Plan.Description, Plan.Price, Plan.DurationDays, Plan.UpdatedAt)
               = (updatePlan.Description, updatePlan.Price, updatePlan.DurationDays, DateTime.Now);

                PlanRepo.Update(Plan);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Plan Failed {ex}");
                return false;
            }
        }

        public bool ToggleStatus(int PlanId)
        {
            var PlanRepo = _unitOfWork.GetRepository<Plan>();
            var Plan = PlanRepo.GetById(PlanId);
            if(Plan is null || HasActiveMemberships(PlanId)) return false;
            try
            {
                Plan.IsActive = Plan.IsActive == true ? false : true;
                Plan.UpdatedAt = DateTime.Now;
                PlanRepo.Update(Plan);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Change Plan Status Failed {ex}");
                return false; 
            }
        }

        #region Helper Methods
        private bool HasActiveMemberships(int PlanId)
        {
            var ActiveMembership = _unitOfWork.GetRepository<Membership>()
                                              .GetAll(X => X.PlanId == PlanId && X.Status == "Active");
            return ActiveMembership.Any();
        }
        #endregion
    }
}
