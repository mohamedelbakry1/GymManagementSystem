using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.TrianerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TrainerController(ITrainerService _trainerService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var Trainers = await _trainerService.GetAllTrainers();

            return View(Trainers);
        }

        public async Task<IActionResult> TrainerDetails(int Id)
        {
            if(Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Trainer can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Trainer = await _trainerService.GetTrainerDetails(Id);

            if(Trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(Trainer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrainer(CreateTrainerViewModel createTrainer)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataInvalid", "Check Data and Missing Fields");
                return RedirectToAction(nameof(Create), createTrainer);
            }

            var result = await _trainerService.CreateTrainer(createTrainer);

            if (result)
                TempData["SuccessMessage"] = "Trainer Created Successfully";
            else
                TempData["ErrorMessage"] = "Trainer Failed To Create, Check Email and Phone";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> TrainerEdit(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Trainer can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Trainer = await _trainerService.GetTrainerToUpdate(Id);

            if (Trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(Trainer);
        }

        [HttpPost]
        public async Task<IActionResult> TrainerEdit([FromRoute] int Id, UpdateTrainerViewModel updateTrainer)
        {
            if (!ModelState.IsValid)
                return View(updateTrainer);

            var result = await _trainerService.UpdateTrainer(Id, updateTrainer);

            if (result)
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
            else
                TempData["ErrorMessage"] = "Trainer Failed To Update";

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Trainer can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Trainer = await _trainerService.GetTrainerDetails(Id);

            if (Trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TrainerId = Trainer.Id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int Id)
        {
            var result = await _trainerService.RemoveTrainer(Id);

            if (result)
                TempData["SuccessMessage"] = "Trainer Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Trainer Can not Delete";

            return RedirectToAction(nameof(Index));
        }
    }
}
