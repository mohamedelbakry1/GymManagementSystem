using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class PlanController(IPlanService _planService) : Controller
    {
        public IActionResult Index()
        {
            var Plans = _planService.GetAllPlans();
            return View(Plans);
        }

        public IActionResult Details(int Id)
        {
            if(Id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Plan Id";
                return RedirectToAction(nameof(Index));
            }

            var Plan = _planService.GetPlanDetails(Id);

            if(Plan is null)
            {
                TempData["ErrorMessage"] = "Plan Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(Plan);
        }

        public IActionResult PlanEdit(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Plan Id";
                return RedirectToAction(nameof(Index));
            }

            var Plan = _planService.GetPlanToUpdate(Id);

            if(Plan is null)
            {
                TempData["ErrorMessage"] = "Plan Can not be Updated";
                return RedirectToAction(nameof(Index));
            }
            return View(Plan);
        }

        [HttpPost]
        public IActionResult PlanEdit([FromRoute]int Id, UpdatePlanViewModel updatePlan)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("WrongData", "Check Data Validation");
                return View(updatePlan);
            }

            var result = _planService.UpdatePlanDetails(Id, updatePlan);

            if (result)
                TempData["SuccessMessage"] = "Plan Updated Successfully";
            else
                TempData["ErrorMessage"] = "Plan Failed To Update";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleStatus([FromRoute] int Id)
        {
            var result = _planService.ToggleStatus(Id);

            if (result)
                TempData["SuccessMessage"] = "Plan Status Changed";
            else
                TempData["ErrorMessage"] = "Failed to Change Plan Status";

            return RedirectToAction(nameof(Index));
        }
    }
}
